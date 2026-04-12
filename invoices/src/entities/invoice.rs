use chrono::{DateTime, Utc};
use uuid::Uuid;

pub struct Invoice {
    pub id: Uuid,
    pub order_id: String,
    pub created_at: DateTime<Utc>,
}

impl Invoice {
    pub fn new(id: Uuid, order_id: String, created_date: DateTime<Utc>) -> Self {
        Self {
            id,
            order_id,
            created_at: created_date,
        }
    }
}
