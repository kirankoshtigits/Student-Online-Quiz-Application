create Table TBL_ADMIN
(   AD_ID int IDENTITY primary key,
	AD_NAME Nvarchar(20) Not null unique,
	AD_PASSWORD Nvarchar(20) not null
)

Create table TBL_QUESTION
(
	QUESTION_ID int IDENTITY primary key,
	Q_TEXT Nvarchar(MAx) NOT NULL,
	OPA Nvarchar(30) NOT NULL unique,
	OPB Nvarchar(30) NOT NULL unique,
	OPC Nvarchar(30) NOT NULL unique,
	OPD Nvarchar(30) NOT NULL unique,
	COP Nvarchar(30) NOT NULL
)

Create table TBL_STUDENT
(
STD_ID int IDENTITY primary key,
STD_NAME Nvarchar(30) NOT NULL unique,
STD_PASSWORD Nvarchar(30) NOT NULL,
STD_Image Nvarchar(MAX) NOT NULL
)

Create table TBL_SETEXAM
(
EXAM_ID  int IDENTITY primary key,
EXAM_DATE DateTime,
EXAM_PK_STU int Foreign key references TBL_STUDENT(STD_ID),  
EXAM_NAME Nvarchar(30) NOT NULL,
EXAN_STD_SCORE int 
)

Create table TBL_CATEGORY
(
CAT_ID  int IDENTITY primary key,
CAT_NAME Nvarchar(50) NOT NULL,
CAT_PK_ADID int Foreign key references TBL_ADMIN(AD_ID), 
)



alter table TBL_QUESTION
add Q_FK_CATID int foreign key references TBL_CATEGORY(CAT_ID)


create table DieryRecord
(
	Person_ID int IDENTITY primary key,
	Person_FirstName varchar(50),
	Person_LastName varchar(50),
	Person_MobileNumber varchar(12),
	Person_HomeNumber varchar(20),
	Person_City varchar(50),
	Person_Tehsil varchar(50),
	Person_District varchar(50),
	Person_State varchar(50),
	R_FK_CATID int foreign key references TBL_CATEGORY(CAT_ID)
)


insert into TBL_ADMIN values ('kirank','kiran12345')