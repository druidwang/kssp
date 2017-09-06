insert into SYS_CodeMstr values('TransportPricingMethod','运单计价方式',0)

insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TransportPricingMethod',0,'CodeDetail_TransportPricingMethod_Chartered',1,1)
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TransportPricingMethod',1,'CodeDetail_TransportPricingMethod_Distance',0,2)
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TransportPricingMethod',2,'CodeDetail_TransportPricingMethod_Weight',0,3)
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TransportPricingMethod',3,'CodeDetail_TransportPricingMethod_Volumn',0,4)
insert into SYS_CodeDet(Code,Value,Desc1,IsDefault,Seq) values ('TransportPricingMethod',4,'CodeDetail_TransportPricingMethod_LadderVolumn',0,5)
