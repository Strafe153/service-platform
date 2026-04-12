use axum::{Router, routing::get};
use futures_lite::stream::StreamExt;
use lapin::{
    Connection, ConnectionProperties,
    options::{BasicAckOptions, BasicConsumeOptions, QueueBindOptions},
    types::FieldTable,
};
use refinery::embed_migrations;
use serde::Deserialize;
use std::{error::Error, fs, sync::Arc};
use tokio_postgres::{Client, NoTls};

use crate::{
    dto::invoice_dto::InvoiceCreateDto,
    handlers::invoice_handler::{get_by_id, get_page},
    models::message_wrapper::MessageWrapper,
    services::invoice_service::InvoiceService,
    state::AppState,
};

#[derive(Debug, Deserialize)]
pub struct Database {
    port: i32,
    host: String,
    user: String,
    password: String,
    name: String,
}

#[derive(Debug, Deserialize)]
pub struct Broker {
    port: i32,
    host: String,
    user: String,
    password: String,
    exchange: Exchange,
}

#[derive(Debug, Deserialize)]
pub struct Exchange {
    name: String,
    queue: String,
    key: String,
}

#[derive(Debug, Deserialize)]
pub struct Config {
    database: Database,
    broker: Broker,
}

impl Config {
    fn queue_connection_string(&self) -> String {
        format!(
            "amqp://{}:{}@{}:{}",
            self.broker.user, self.broker.password, self.broker.host, self.broker.port
        )
    }

    fn db_connection_string(&self) -> String {
        format!(
            "host={} port={} user={} password={} dbname={}",
            self.database.host,
            self.database.port,
            self.database.user,
            self.database.password,
            self.database.name
        )
    }
}

const CONFIG_PATH: &'static str = "./config.toml";

pub fn read_config() -> Result<Config, Box<dyn Error>> {
    let config_str = fs::read_to_string(CONFIG_PATH)?;
    let config = toml::from_str(&config_str)?;

    Ok(config)
}

pub async fn configure_database(config: &Config) -> Result<Client, Box<dyn Error>> {
    let connection_string = config.db_connection_string();

    let (mut client, connection) = tokio_postgres::connect(&connection_string, NoTls).await?;

    tokio::spawn(async move {
        if let Err(e) = connection.await {
            eprintln!("Connection error: {:?}", e);
        }
    });

    embed_migrations!("./src/migrations");
    migrations::runner().run_async(&mut client).await?;

    Ok(client)
}

pub async fn configure_messaging(
    config: &Config,
    service: Arc<dyn InvoiceService>,
) -> Result<(), Box<dyn Error>> {
    let connection_string = config.queue_connection_string();
    let runtime = lapin::runtime::default_runtime()?;

    let conn = Connection::connect_with_runtime(
        &connection_string,
        ConnectionProperties::default().with_connection_name("invoices".into()),
        runtime,
    )
    .await?;

    let channel = conn.create_channel().await?;

    channel
        .queue_bind(
            config.broker.exchange.queue.as_str().into(),
            config.broker.exchange.name.as_str().into(),
            config.broker.exchange.key.as_str().into(),
            QueueBindOptions::default(),
            FieldTable::default(),
        )
        .await?;

    let mut consumer = channel
        .basic_consume(
            config.broker.exchange.queue.as_str().into(),
            config.broker.exchange.key.as_str().into(),
            BasicConsumeOptions::default(),
            FieldTable::default(),
        )
        .await?;

    tokio::spawn(async move {
        while let Some(delivery_result) = consumer.next().await {
            match delivery_result {
                Ok(delivery) => {
                    let message: Result<MessageWrapper<InvoiceCreateDto>, serde_json::Error> =
                        serde_json::from_slice(&delivery.data);

                    match message {
                        Ok(dto) => {
                            if let Err(e) = service.create(dto.message).await {
                                println!("{}", e);
                                continue;
                            }
                        }
                        Err(e) => eprintln!("Deserialization error: {:?}", e),
                    }

                    if let Err(e) = delivery.ack(BasicAckOptions::default()).await {
                        eprintln!("Acknowledgment error: {:?}", e);
                    }
                }
                Err(e) => eprintln!("Consumer error: {:?}", e),
            }
        }
    });

    Ok(())
}

pub async fn configure_http(service: Arc<dyn InvoiceService>) -> Result<(), Box<dyn Error>> {
    let state = Arc::new(AppState::new(service));

    let app = Router::new()
        .route("/invoices", get(get_page))
        .route("/invoices/{id}", get(get_by_id))
        .with_state(state);

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3000").await?;

    axum::serve(listener, app).await?;

    Ok(())
}
