-- Tạo tất cả bảng SQL cho hệ thống quản lý nông sản - SQL Server Version
-- 1. Roles
CREATE TABLE Roles (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVarchar(50) NOT NULL UNIQUE -- chỉ tồn tại 1 name role
);

-- 2. Users
CREATE TABLE Users (
    id INT PRIMARY KEY IDENTITY(1,1),
    username NVarchar(100) UNIQUE NOT NULL, -- check trùng username
    password NVarchar(255) NOT NULL,
    email NVarchar(100) UNIQUE NOT NULL, -- check trùng email
    role_id INT NOT NULL,
    HasAcceptedGeolocation BIT NOT NULL DEFAULT 0,
    Phone Nvarchar(10) UNIQUE NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (role_id) REFERENCES Roles(id)
);
-- 3. Farmer
CREATE TABLE Farmer (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    full_name NVarchar(100),
    phone NVarchar(20) UNIQUE NOT NULL, -- check trùng
	CONSTRAINT CK_Farmer_Phone_OnlyDigits CHECK (phone NOT LIKE '%[^0-9]%'),
    address TEXT NOT NULL, -- bắt buộc phải có
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 4. Shipper
CREATE TABLE Shipper (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    full_name NVarchar(100),
    phone NVarchar(20) UNIQUE NOT NULL, -- check trùng
	CONSTRAINT CK_Shipper_Phone_OnlyDigits CHECK (phone NOT LIKE '%[^0-9]%'),
    vehicle_info TEXT, -- bắt buộc phải có
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 5. ProductCategory
CREATE TABLE ProductCategory (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVarchar(100) UNIQUE, -- không đc trùng danh mục
	imageUrl NVarchar(100)
);

-- 6. Product
CREATE TABLE Product (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVarchar(100) UNIQUE NOT NULL, -- không trùng tên hàng hóa
    category_id INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    expiration_date DATE,
    product_type NVarchar(20) CHECK (product_type IN ('processed', 'raw')),
    quantity INT NOT NULL,
    processing_time DATE,
    farmer_id INT,
    FOREIGN KEY (category_id) REFERENCES ProductCategory(id),
    FOREIGN KEY (farmer_id) REFERENCES Farmer(id)
);

-- 1 hàng có nhiều ảnh
CREATE TABLE ProductImage (
    id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT,
    image_url NVarchar(255),
    description TEXT,
    uploaded_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);


-- 8. Stock kho hàng
CREATE TABLE Stock (
    id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT,
    quantity INT,
    last_updated DATETIME,
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

-- 9. hóa đơn nhập khẩu
CREATE TABLE ImportInvoice (
    id INT PRIMARY KEY IDENTITY(1,1),
    supplier_name NVarchar(100),
    total_amount DECIMAL(12,2),
    created_at DATETIME,
	purchase_time DATETIME,
);

-- 10. chi tiết hóa đơn nhập khảu
CREATE TABLE ImportInvoiceDetail (
    id INT PRIMARY KEY IDENTITY(1,1),
    import_invoice_id INT,
    product_id INT,
    quantity INT,
    unit_price DECIMAL(10,2),
    FOREIGN KEY (import_invoice_id) REFERENCES ImportInvoice(id),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

-- 13. đơn bán lẻ
CREATE TABLE RetailOrder (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    order_date DATETIME,
    status NVarchar(50) CHECK (status IN ('pending', 'confirmed', 'shipped', 'delivered', 'cancelled', 'returned')),
	confirmed_at DATETIME, -- xác nhận đơn hàng
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 14. RetailOrderItem
CREATE TABLE RetailOrderItem (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT,
    product_id INT,
    quantity INT,
    unit_price DECIMAL(10,2),
    FOREIGN KEY (order_id) REFERENCES RetailOrder(id),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

-- 15. Cart
CREATE TABLE Cart (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT UNIQUE,
    created_at DATETIME,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 16. CartItem
CREATE TABLE CartItem (
    id INT PRIMARY KEY IDENTITY(1,1),
    cart_id INT,
    product_id INT,
    quantity INT,
    FOREIGN KEY (cart_id) REFERENCES Cart(id),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

-- 17. Delivery -- bỏ Delivery_status bảng này chỉ những đơn deli r mới lưu vào đây ko cần status
CREATE TABLE Delivery (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_type NVarchar(10) CHECK (order_type IN ('retail', 'import')),
    order_id INT,
    shipper_id INT,
    shipping_fee DECIMAL(10,2),
    start_time DATETIME,
    end_time DATETIME,
    FOREIGN KEY (shipper_id) REFERENCES Shipper(id)
);

-- 18. ProcessingOrder
CREATE TABLE ProcessingOrder (
    id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT,
    quantity INT,
    send_date DATE,
    expected_return_date DATE,
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

-- 19. ProcessingReceipt
CREATE TABLE ProcessingReceipt (
    id INT PRIMARY KEY IDENTITY(1,1),
    processing_order_id INT,
    received_date DATE,
    actual_quantity INT,
    FOREIGN KEY (processing_order_id) REFERENCES ProcessingOrder(id)
);

-- tách ra vì có thể 1 sản phẩm chế biến đc nhiều lần
-- dùng để lưu lịch sử của đơn chế biến
CREATE TABLE ProductProcessingHistory (
    id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT,
    sent_date DATE,
    return_date DATE,
    quantity INT,
    FOREIGN KEY (product_id) REFERENCES Product(id)
);


-- 20. ProcessedProduct - dùng để lưu tất cả kết quả 
CREATE TABLE ProcessedProduct (
    id INT PRIMARY KEY IDENTITY(1,1),
    product_id INT,
	history_id INT,
    processed_date DATE,
    return_date DATE,
    processing_note TEXT,
	total_weight DECIMAL(10,2),
	image_url NVarchar(255),
	description TEXT,
    FOREIGN KEY (history_id) REFERENCES ProductProcessingHistory(id)
);

-- 21. Feedback chưa tất cả các đánh giá kể cả nhập kho hay xuất kho
CREATE TABLE Feedback (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    order_id INT,
    order_type NVarchar(10) CHECK (order_type IN ('retail', 'import')),
    content TEXT,
    rating INT,
    created_at DATETIME,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 22. Address
CREATE TABLE Address (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    address_line TEXT,
    city NVarchar(100),
    province NVarchar(100),
    postal_code NVarchar(20),
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 23. Payment cả thanh toán cho nhập kho hay xuất kho hoàn hàng
CREATE TABLE Payment (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    order_id INT,
    order_type NVarchar(10) CHECK (order_type IN ('retail', 'import')),
    amount DECIMAL(10,2),
    paid_at DATETIME,
    method NVarchar(50) CHECK (method IN ('cash', 'card', 'bank_transfer')),
	payment_type NVarchar(10) CHECK (payment_type IN ('receive', 'refund'))
	-- chỉ nhận tiền mặt chuyển khoản hoặc qua visa
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 24. TransportRoute
CREATE TABLE TransportRoute (
    id INT PRIMARY KEY IDENTITY(1,1),
    start_location TEXT,
    end_location TEXT,
    distance_km DECIMAL(10,2),
    estimated_time NVarchar(50)
);

-- 25. Notification
CREATE TABLE Notification (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    content TEXT,
    created_at DATETIME,
    is_read BIT DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 26. Log
CREATE TABLE Log (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    action TEXT,
    created_at DATETIME,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 27. Session
CREATE TABLE Session (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    session_token TEXT,
    created_at DATETIME,
    expires_at DATETIME,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);

-- 28. Report
CREATE TABLE Report (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVarchar(100),
    content TEXT,
    created_at DATETIME
);

-- 29. Staff
CREATE TABLE Staff (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    role NVarchar(100),
    FOREIGN KEY (user_id) REFERENCES Users(id),
);

-- 30. Supplier
CREATE TABLE Supplier (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVarchar(100) UNIQUE NOT NULL, -- check trùng
    contact_info TEXT NOT NULL
);

-- hoàn hàng 
CREATE TABLE ReturnOrder (
    id INT PRIMARY KEY IDENTITY(1,1),
    order_type NVarchar(10) CHECK (order_type IN ('retail', 'import')),
    order_id INT, -- bắt buộc phải kiểm tra trong code order_type 
    user_id INT,
    quantity INT,
    reason TEXT,
    created_at DATETIME,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);
------------------------------------ADD Table--------------------------------------------------------------

CREATE TABLE SupplyRequest (
    id INT PRIMARY KEY IDENTITY(1,1),
    
    requester_type NVARCHAR(10) CHECK (requester_type IN ('admin', 'farmer')) NOT NULL,

    requester_id INT NOT NULL,   
    receiver_id INT NOT NULL,   

    farmer_id INT NOT NULL,     

    product_name NVARCHAR(100) NOT NULL,
    quantity INT NOT NULL,
    price DECIMAL(10,2), -- giá đề xuất

    status NVARCHAR(20) CHECK (status IN ('pending', 'accepted', 'rejected')) DEFAULT 'pending',

    requested_at DATETIME DEFAULT GETDATE(),
    responded_at DATETIME NULL,

    FOREIGN KEY (requester_id) REFERENCES Users(id),
    FOREIGN KEY (receiver_id) REFERENCES Users(id),
    FOREIGN KEY (farmer_id) REFERENCES Farmer(id),
);

CREATE TABLE FarmerRegistrationRequest (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,	
    full_name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20) UNIQUE NOT NULL,
    address TEXT NOT NULL,
    status NVARCHAR(20) CHECK (status IN ('pending', 'approved', 'rejected')) DEFAULT 'pending',
    requested_at DATETIME DEFAULT GETDATE(),
    reviewed_at DATETIME NULL,
    reviewed_by INT NULL, -- admin user_id
    FOREIGN KEY (user_id) REFERENCES Users(id),
    FOREIGN KEY (reviewed_by) REFERENCES Users(id)
);

CREATE TABLE UserLocations (
    Id INT PRIMARY KEY IDENTITY,
    UserId int NOT NULL,
    Label NVARCHAR(100),             
    Address NVARCHAR(255),           
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
------------------------------------INSERT--------------------------------------------------------------

INSERT INTO Roles (name) VALUES
('admin'),
('staff'),
('customer'),
('shipper'),
('farmer');

INSERT INTO Users (username, password, email, role_id, HasAcceptedGeolocation, Phone)
VALUES (N'admin', N'admin123', N'admin@example.com', 1, 0, 0123456789);

select * from Users

INSERT INTO ProductCategory (name, imageUrl) VALUES
(N'Vegetables & Fruit', N'back-end/svg/vegetable.svg'),
(N'Beverages', N'back-end/svg/cup.svg'),
(N'Meats & Seafood', N'back-end/svg/meats.svg'),
(N'Breakfast', N'back-end/svg/breakfast.svg'),
(N'Frozen Foods', N'back-end/svg/frozen.svg'),
(N'Milk & Dairies', N'back-end/svg/milk.svg'),
(N'Pet Food', N'back-end/svg/pet.svg');

INSERT INTO Users (username, password, email, role_id, created_at)
VALUES 
(N'customer01', N'cus123', N'customer01@example.com', 3, GETDATE()),
(N'customer02', N'cus456', N'customer02@example.com', 3, GETDATE()),
(N'customer03', N'cus789', N'customer03@example.com', 3, GETDATE()),
(N'customer04', N'cusabc', N'customer04@example.com', 3, GETDATE()),
(N'customer05', N'cusxyz', N'customer05@example.com', 3, GETDATE());

SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Shipper'