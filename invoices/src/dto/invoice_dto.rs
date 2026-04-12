use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use uuid::Uuid;

use crate::entities::invoice::Invoice;

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct InvoiceReadDto {
    id: Uuid,
    order_id: String,
    created_at: DateTime<Utc>,
}

impl From<Invoice> for InvoiceReadDto {
    fn from(value: Invoice) -> Self {
        Self {
            id: value.id,
            order_id: value.order_id,
            created_at: value.created_at,
        }
    }
}

#[derive(Debug, Deserialize)]
pub struct InvoiceCreateDto {
    #[serde(rename = "orderId")]
    pub order_id: String,
    #[serde(rename = "completedAt")]
    pub created_at: DateTime<Utc>,
}
