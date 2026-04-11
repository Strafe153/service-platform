use std::error::Error;

use tokio;

use crate::{
    config::{configure_axum, configure_database, configure_messaging},
    models::page::Page,
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

    configure_messaging(&config).await?;
    configure_axum(client).await?;

    Ok(())
}
