use chrono::NaiveDateTime;
use uuid::Uuid;

pub struct Invoice {
    pub id: Uuid,
    pub order_id: String,
    pub created_at: NaiveDateTime,
}

impl Invoice {
    pub fn new(id: Uuid, order_id: String, created_date: NaiveDateTime) -> Self {
        Self {
            id,
            order_id,
            created_at: created_date,
        }
    }
}
