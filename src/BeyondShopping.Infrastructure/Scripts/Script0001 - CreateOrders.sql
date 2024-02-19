CREATE TABLE orders
(
    id serial NOT NULL,
    user_id integer NOT NULL,
    status character varying NOT NULL,
    created_at timestamp without time zone NOT NULL,
    PRIMARY KEY (id)
);

CREATE INDEX idx_orders_user_id ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_created_at ON orders(created_at);