CREATE DATABASE IF NOT EXISTS NorthwindTraders
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE NorthwindTraders;

CREATE TABLE IF NOT EXISTS Customers (
    CustomerId  VARCHAR(5)  NOT NULL,
    CompanyName VARCHAR(40) NOT NULL,
    ContactName VARCHAR(30) NULL,
    City        VARCHAR(15) NULL,
    Country     VARCHAR(15) NULL,
    PRIMARY KEY (CustomerId)
);

CREATE TABLE IF NOT EXISTS Products (
    ProductId   INT          NOT NULL AUTO_INCREMENT,
    ProductName VARCHAR(40)  NOT NULL,
    UnitPrice   DECIMAL(10,2) NULL,
    PRIMARY KEY (ProductId)
);

CREATE TABLE IF NOT EXISTS Orders (
    OrderId    INT         NOT NULL AUTO_INCREMENT,
    CustomerId VARCHAR(5)  NULL,
    OrderDate  DATETIME    NULL,
    PRIMARY KEY (OrderId),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);

CREATE TABLE IF NOT EXISTS OrderDetails (
    OrderId   INT            NOT NULL,
    ProductId INT            NOT NULL,
    UnitPrice DECIMAL(10,2)  NOT NULL,
    Quantity  SMALLINT       NOT NULL,
    Discount  FLOAT          NOT NULL DEFAULT 0,
    PRIMARY KEY (OrderId, ProductId),
    FOREIGN KEY (OrderId)   REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- Seed data
INSERT INTO Customers (CustomerId, CompanyName, ContactName, City, Country) VALUES
('ALFKI', 'Alfreds Futterkiste',       'Maria Anders',   'Berlin',    'Germany'),
('ANATR', 'Ana Trujillo Emparedados',  'Ana Trujillo',   'Mexico D.F.','Mexico'),
('ANTON', 'Antonio Moreno Taqueria',   'Antonio Moreno', 'Mexico D.F.','Mexico'),
('AROUT', 'Around the Horn',           'Thomas Hardy',   'London',    'UK'),
('BERGS', 'Berglunds snabbköp',        'Christina Berglund','Luleå', 'Sweden'),
('BLAUS', 'Blauer See Delikatessen',   'Hanna Moos',     'Mannheim',  'Germany'),
('BOLID', 'Bólido Comidas preparadas', 'Martín Sommer',  'Madrid',    'Spain'),
('BONAP', 'Bon app''',                 'Laurence Lebihan','Marseille', 'France');

INSERT INTO Products (ProductName, UnitPrice) VALUES
('Chai',               18.00),
('Chang',              19.00),
('Aniseed Syrup',      10.00),
('Chef Anton''s Cajun Seasoning', 22.00),
('Grandma''s Boysenberry Spread', 25.00),
('Uncle Bob''s Organic Dried Pears', 30.00),
('Northwoods Cranberry Sauce', 40.00),
('Mishi Kobe Niku',   97.00);

INSERT INTO Orders (CustomerId, OrderDate) VALUES
('ALFKI', '2024-01-15'),
('ALFKI', '2024-03-22'),
('ANATR', '2024-02-10'),
('AROUT', '2024-04-05'),
('AROUT', '2024-05-18'),
('BERGS', '2024-01-30'),
('BONAP', '2024-06-01');

INSERT INTO OrderDetails (OrderId, ProductId, UnitPrice, Quantity, Discount) VALUES
(1, 1, 18.00, 10, 0),
(1, 3, 10.00,  5, 0.05),
(2, 2, 19.00,  3, 0),
(2, 5, 25.00,  2, 0.1),
(3, 4, 22.00,  4, 0),
(4, 6, 30.00,  6, 0),
(4, 7, 40.00,  1, 0),
(4, 8, 97.00,  2, 0.15),
(5, 1, 18.00,  5, 0),
(6, 2, 19.00, 12, 0),
(6, 3, 10.00,  8, 0.05),
(7, 5, 25.00,  3, 0);
