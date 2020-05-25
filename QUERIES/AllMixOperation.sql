select * from Sys.tables

insert into TBL_ADMIN values('AdminData','Amin1232')


alter table TBL_QUESTION
add Q_FK_CATID int foreign key references TBL_CATEGORY(CAT_ID)

select * from TBL_ADMIN
select * from TBL_CATEGORY