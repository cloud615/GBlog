create table permissioninfo
(
	permissionid int primary key ,
	name varchar(64) not null,
	parentid int default 0,
	ctrlid varchar(64),
	controllername varchar(64),
	actionname varchar(64),
	pageurl varchar(256),
	remark varchar(128),
	remark2 varchar(128)
)
create table roleinfo
(
	roleid int primary key,
	name varchar(64) not null,
	permissionitems varchar(1024),
	remark varchar(128)
)

create table userinfo
(
	userid int primary key,
	username varchar(64) not null,
	pwd varchar(64) not null,
	roleitems varchar(512),
	utype int default 0,
	email varchar(256),
	personname varchar(64) not null
)