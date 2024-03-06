# PolarBearEapApi



## Description
對產線的設備提供一個REST API介面，使其能與後端的MES系統溝通。<P>
目錄:
- PolarBearEapApi : REST API 程式
- PolarBearEapApiTests : Unit Test
- IntegrationTest : Functional Test 

所有API的Input都是以下的json格式 (格式主要為了相容之前設備商的程式): <P>
{
"Hwd":"36bd2cd3-3c94-4d53-a494-79eab5d34e9f",
"Indicator":"QUERY_RECORD",
"SerializeData":"{"LineCode":"E08-1FT-01","SectionCode":"T04A-STATION432","StationCode":72617,"OPCategory":"UNIT_PROCESS_CHECK","OPRequestInfo":{"SN":"J21GYM001M100000US"},"OPResponseInfo":{}}"
}

- Hwd: Token
- SerializeData.LineCode/SectionCode/StationCode : 產線設備的indentifier
- SerializeData.OPCategory : 操作類別(動作)
- SerializeData.OPRequestInfo : Input參數
- SerializeData.OPResponseInfo : 執行結果


