use async_trait::async_trait;
use std::error::Error;
use tokio_postgres::Client;
use uuid::Uuid;

use crate::entities::invoice::Invoice;

#[async_trait]
pub trait InvoiceRepository {
    async fn get(&self, skip: i64, take: i64) -> Result<Vec<Invoice>, Box<dyn Error>>;
    async fn get_by_id(&self, id: Uuid) -> Result<Invoice, Box<dyn Error>>;
}

pub struct PostgresInvoiceRepository {
    pub client: Client,
}

impl PostgresInvoiceRepository {
    pub fn new(client: Client) -> Self {
        Self { client }
    }
}

#[async_trait]
impl InvoiceRepository for PostgresInvoiceRepository {
    async fn get(&self, skip: i64, take: i64) -> Result<Vec<Invoice>, Box<dyn Error>> {
        let rows = self
            .client
            .query(
                "SELECT id, orderId, createdAt FROM invoices LIMIT $1 OFFSET $2",
                &[&take, &skip],
            )
            .await?;

        let invoices: Vec<Invoice> = rows
            .iter()
            .map(|r| Invoice::new(r.get(0), r.get(1), r.get(2)))
            .collect();

        Ok(invoices)
    }

    async fn get_by_id(&self, id: Uuid) -> Result<Invoice, Box<dyn Error>> {
        let row = self
            .client
            .query_one(
                "SELECT id, orderId, createdAt FROM invoices WHERE id = $1",
                &[&id],
            )
            .await?;

        // most likely check if row exists and return not found error and map that to a not found,
        // otherwise bad request

        let invoice = Invoice::new(row.get(0), row.get(1), row.get(2));

        Ok(invoice)
    }
}
