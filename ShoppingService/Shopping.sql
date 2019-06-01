--drop table Products;

Create table Products
(P_ID int identity(1,1)
,P_Name varchar(50) not null
,P_Price decimal(15,2) not null
,constraint Pk_Product_ID primary key (P_ID)
,constraint UQ_Product_Name unique (P_Name)
);

insert into products values ('Milk',14.90);
insert into products values ('Bread',35.50);
insert into products values ('Lettuce',11.80);
insert into products values ('Cheese',49.45);

select * from products;

--drop table Users;

Create table Users
(U_ID int identity(1,1)
,U_Name varchar(50) not null
,U_Gender char(1) not null
,constraint PK_User_ID primary key (U_ID)
,constraint UQ_Users_Name unique (U_Name), 
constraint Chk_User_Gender check(U_Gender IN ('F','M'))
);

insert into Users values ('John','M');
insert into Users values ('Sansa','F');

select * from Users;

--drop table Orders;

Create table Orders
(O_ID int identity(1,1)
,O_UserID int not null
,O_TotalAmount decimal(15,2) not null
,O_Status char(1) not null
,constraint PK_Order_ID primary key (O_ID)
,constraint Chk_Status check (O_Status in ('O','C'))
);

select * from Orders;

--drop table Cart;

Create table Cart
(C_ID int identity(1,1)
,C_OrderID int not null
,C_ProductID int not null
,C_Count int not null
,constraint PK_Cart_ID primary key (C_ID)
,constraint UQ_Order_Product unique (C_OrderID,C_ProductID)
);

select * from Cart;

-- drop table Discount

Create table Discount
(D_ID int identity(1,1)
,D_ProductID int not null
,D_DiscountCode varchar(10) not null
,D_Multiplier decimal(15,2) not null
,constraint PK_D_ID primary key(D_ID)
);

insert into Discount values (1,'3FOR2',0.67);
insert into Discount values (0,'ALL10',0.10);

select * from Discount;
--update Discount set D_Multiplier = 0.33 where D_ID=1;
--update Discount set D_Multiplier = 0.10 where D_ID=2;

