use chrono::NaiveDateTime;
use serde::Serialize;
use uuid::Uuid;

use crate::entities::invoice::Invoice;

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct InvoiceReadDto {
    id: Uuid,
    order_id: String,
    created_at: NaiveDateTime,
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
