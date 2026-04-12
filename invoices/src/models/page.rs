use serde::Deserialize;

#[derive(Deserialize)]
pub struct Page {
    #[serde(default = "number")]
    number: i64,
    #[serde(default = "size")]
    size: i64,
}

impl Page {
    pub fn get_skip_count(&self) -> i64 {
        (self.number - 1) * self.size
    }

    pub fn get_number(&self) -> i64 {
        self.number
    }
}

fn number() -> i64 {
    1
}

fn size() -> i64 {
    10
}
