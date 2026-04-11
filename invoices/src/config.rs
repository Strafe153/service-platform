use axum::{Router, routing::get};
use lapin::{Connection, ConnectionProperties, options::QueueBindOptions, types::FieldTable};
use refinery::embed_migrations;
use serde::Deserialize;
use std::{error::Error, fs, sync::Arc};
use tokio_postgres::{Client, NoTls};

use crate::{
    handlers::invoice_handler::{get_by_id, get_page},
    repositories::invoice_repository::PostgresInvoiceRepository,
    services::invoice_service::InvoiceServiceImpl,
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
pub struct Queue {
    port: i32,
    host: String,
    user: String,
    password: String,
}

#[derive(Debug, Deserialize)]
pub struct Config {
    database: Database,
    queue: Queue,
}

impl Config {
    fn queue_connection_string(&self) -> String {
        format!(
            "amqp://{}:{}@{}:{}",
            self.queue.user, self.queue.password, self.queue.host, self.queue.port
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
            println!("Connection error: {}", e);
        }
    });

    embed_migrations!("./src/migrations");
    migrations::runner().run_async(&mut client).await?;

    Ok(client)
}

pub async fn configure_messaging(config: &Config) -> Result<(), Box<dyn Error>> {
    let conn_string = config.queue_connection_string();

    let runtime = lapin::runtime::default_runtime()?;

    let conn = Connection::connect_with_runtime(
        &conn_string,
        ConnectionProperties::default().with_connection_name("invoices".into()),
        runtime,
    )
    .await?;

    let ch = conn.create_channel().await?;

    ch.queue_bind(
        "order-completed".into(),
        "order".into(),
        "order.completed".into(),
        QueueBindOptions::default(),
        FieldTable::default(),
    )
    .await?;

    // yet to finish

    Ok(())
}

pub async fn configure_axum(client: Client) -> Result<(), Box<dyn Error>> {
    let state = Arc::new(AppState::new(InvoiceServiceImpl::new(
        PostgresInvoiceRepository::new(client),
    )));

    let app = Router::new()
        .route("/invoices", get(get_page))
        .route("/invoices/{id}", get(get_by_id))
        .with_state(state);

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3000").await?;

    axum::serve(listener, app).await?;

    Ok(())
}
