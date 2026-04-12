use std::{error::Error, sync::Arc};

use tokio;

use crate::{
    config::{configure_database, configure_http, configure_messaging},
    models::page::Page,
    repositories::invoice_repository::PostgresInvoiceRepository,
    services::invoice_service::InvoiceServiceImpl,
    state::AppState,
};

mod config;
mod dto;
mod entities;
mod handlers;
mod models;
mod repositories;
mod services;
mod state;

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    let config = config::read_config()?;

    let client = configure_database(&config).await?;
    let service = Arc::new(InvoiceServiceImpl {
        repository: PostgresInvoiceRepository { client },
    });

    configure_messaging(&config, service.clone()).await?;
    configure_http(service.clone()).await?;

    Ok(())
}
