CREATE TABLE orders_items
(
    order_id integer NOT NULL,
    item_id integer NOT NULL,
    quantity integer NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id)
);

CREATE INDEX idx_orders_items_order_id ON orders_items(order_id);