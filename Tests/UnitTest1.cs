using Microsoft.VisualStudio.TestTools.UnitTesting;
using PivotDataTable;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static Tests.UnitTest1;

namespace Tests
{

	[TestClass]
	public class UnitTest1
	{

		public class Test1Row
		{
			public string Site { get; set; }
			public string Unit { get; set; }
			public string Group { get; set; }
			public string Name { get; set; }
			public int Number { get; set; }
			public double Weight { get; set; }
		}

		const string str_TestCompareFastAndSlow_RowGroupOnSite = """
			<DocumentElement>
			  <row>
			    <Site>Site1</Site>
			    <Unit>Unit1, Unit2</Unit>
			    <Group>Group1, Group2</Group>
			    <Name>Name1, Name3</Name>
			    <Number>7</Number>
			    <Weight>3.6999999999999997</Weight>
			    <RowCount>3</RowCount>
			  </row>
			  <row>
			    <Site>Site3</Site>
			    <Unit>Unit1</Unit>
			    <Group>Group1</Group>
			    <Name>Name1</Name>
			    <Number>5</Number>
			    <Weight>2.1</Weight>
			    <RowCount>1</RowCount>
			  </row>
			  <row>
			    <Site>Site5</Site>
			    <Unit>Unit6</Unit>
			    <Group>Group1</Group>
			    <Name>Name1</Name>
			    <Number>6</Number>
			    <Weight>5.1</Weight>
			    <RowCount>1</RowCount>
			  </row>
			</DocumentElement>
			""";

		const string str_TestCompareFastAndSlow_RowGroupOnSite_json = """
			{
			  "rows": [
			    [
			      "Site1",
			      "Unit1, Unit2",
			      "Group1, Group2",
			      "Name1, Name3",
			      7,
			      3.6999999999999997,
			      3
			    ],
			    [
			      "Site3",
			      "Unit1",
			      "Group1",
			      "Name1",
			      5,
			      2.1,
			      1
			    ],
			    [
			      "Site5",
			      "Unit6",
			      "Group1",
			      "Name1",
			      6,
			      5.1,
			      1
			    ]
			  ]
			}
			""";

		const string str_TestCompareFastAndSlow_GetTable_DictArr = """
			{
			  "rows": [
			    {
			      "Site": "Site1",
			      "Unit": "Unit1, Unit2",
			      "Group": "Group1, Group2",
			      "Name": "Name1, Name3",
			      "Number": 7,
			      "Weight": 3.6999999999999997,
			      "RowCount": 3
			    },
			    {
			      "Site": "Site3",
			      "Unit": "Unit1",
			      "Group": "Group1",
			      "Name": "Name1",
			      "Number": 5,
			      "Weight": 2.1,
			      "RowCount": 1
			    },
			    {
			      "Site": "Site5",
			      "Unit": "Unit6",
			      "Group": "Group1",
			      "Name": "Name1",
			      "Number": 6,
			      "Weight": 5.1,
			      "RowCount": 1
			    }
			  ]
			}
			""";

		const string nested_TestCompareFastAndSlow_RowGroupOnSite = """
			{
			  "rows": [
			    {
			      "Site": "Site1",
			      "Unit": "Unit1, Unit2",
			      "Group": "Group1, Group2",
			      "Name": "Name1, Name3",
			      "Number": 7,
			      "Weight": 3.6999999999999997,
			      "RowCount": 3
			    },
			    {
			      "Site": "Site3",
			      "Unit": "Unit1",
			      "Group": "Group1",
			      "Name": "Name1",
			      "Number": 5,
			      "Weight": 2.1,
			      "RowCount": 1
			    },
			    {
			      "Site": "Site5",
			      "Unit": "Unit6",
			      "Group": "Group1",
			      "Name": "Name1",
			      "Number": 6,
			      "Weight": 5.1,
			      "RowCount": 1
			    }
			  ]
			}
			""";

		const string TestCompareFastAndSlow_RowGroupOnSite_csv_slow = """
			Site;Unit;Group;Name;Number;Weight;RowCount
			Site1;Unit1, Unit2;Group1, Group2;Name1, Name3;7;3.6999999999999997;3
			Site3;Unit1;Group1;Name1;5;2.1;1
			Site5;Unit6;Group1;Name1;6;5.1;1
			
			""";

		const string TestCompareFastAndSlow_RowGroupOnSite_csv_flat = """
			Site;Unit;Group;Name;Number;Weight;RowCount
			Site1;Unit1, Unit2;Group1, Group2;Name1, Name3;7;3.6999999999999997;3
			Site3;Unit1;Group1;Name1;5;2.1;1
			Site5;Unit6;Group1;Name1;6;5.1;1

			""";

		//		const string TestCompareFastAndSlow_RowGroupOnSite_nestcsv = @"Site;Unit;Group;Name;Number;Weight;RowCount
		//Site1;Unit1, Unit2;Group1, Group2;Name1, Name3;7;3.6999999999999997;3
		//Site3;Unit1;Group1;Name1;5;2.1;1
		//Site5;Unit6;Group1;Name1;6;5.1;1
		//";

		[TestMethod]
		public void TestCompareFastAndSlow_RowGroupOnSite()
		{
			Pivoter<Test1Row> p = GetPivoterTestData();

			var fields = p.Fields.ToDictionary(k => k.Name);
			fields[nameof(Test1Row.Site)].Area = Area.Row;
			fields[nameof(Test1Row.Site)].SortOrder = SortOrder.Asc;

			fields[nameof(Test1Row.Number)].SortOrder = SortOrder.Asc;

			var gdata_fis = p.GetGroupedData_FastIntersect();
			var gdata_ptb = p.GetGroupedData_PivotTableBuilder();

			var pres_fis = new Presentation<Test1Row>(gdata_fis);
			var pres_ptb = new Presentation2<Test1Row>(gdata_ptb);

			var dt_fis = pres_fis.GetDataTable();
			var dt_ptb = pres_ptb.GetDataTable();

			string xml_ptb = dt_ptb.ToXml();
			string xml_fis = dt_fis.ToXml();

			Assert.AreEqual(str_TestCompareFastAndSlow_RowGroupOnSite, xml_ptb);
			Assert.AreEqual(xml_ptb, xml_fis);

			var table_fis = pres_fis.GetTable_Array();
			string json_fis = ToJson(table_fis);
			string json_ptb = ToJson(pres_ptb.GetTable_Array());
			Assert.AreEqual(str_TestCompareFastAndSlow_RowGroupOnSite_json, json_fis);
			Assert.AreEqual(json_fis, json_ptb);

			var csv_fis = table_fis.ToCsv();
			Assert.AreEqual(TestCompareFastAndSlow_RowGroupOnSite_csv_slow, csv_fis);

			var tblDictArr_fis = pres_fis.GetTable_FlatKeyValueList_CompleteColumns();
			var json_tblDictArr_fis = ToJson(tblDictArr_fis);
			Assert.AreEqual(str_TestCompareFastAndSlow_GetTable_DictArr, json_tblDictArr_fis);
			var flatCsv_fis = tblDictArr_fis.ToCsv();
			Assert.AreEqual(TestCompareFastAndSlow_RowGroupOnSite_csv_flat, flatCsv_fis);

			// same as GetTable_FlatDict in this case
			var nested_fis = pres_fis.GetTable_NestedKeyValueList_VariableColumns();
			var nest_json_fis = ToJson(nested_fis);
			Assert.AreEqual(nested_TestCompareFastAndSlow_RowGroupOnSite, nest_json_fis);
//			var nestCsv = nested.ToCsv();
//			Assert.Equal(TestCompareFastAndSlow_RowGroupOnSite_nestcsv, nestCsv);
		}

		private static string ToJson<T>(T table)
		{
			return JsonSerializer.Serialize<T>(table, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		}

		const string str_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName = """
			<DocumentElement>
			  <row>
			    <Site>Site1</Site>
			    <_x002F_Name_x003A_Name1_x002F_Unit>Unit1</_x002F_Name_x003A_Name1_x002F_Unit>
			    <_x002F_Name_x003A_Name1_x002F_Group>Group1, Group2</_x002F_Name_x003A_Name1_x002F_Group>
			    <_x002F_Name_x003A_Name1_x002F_Number>3</_x002F_Name_x003A_Name1_x002F_Number>
			    <_x002F_Name_x003A_Name1_x002F_Weight>2.3</_x002F_Name_x003A_Name1_x002F_Weight>
			    <_x002F_Name_x003A_Name1_x002F_RowCount>2</_x002F_Name_x003A_Name1_x002F_RowCount>
			    <_x002F_Name_x003A_Name3_x002F_Unit>Unit2</_x002F_Name_x003A_Name3_x002F_Unit>
			    <_x002F_Name_x003A_Name3_x002F_Group>Group1</_x002F_Name_x003A_Name3_x002F_Group>
			    <_x002F_Name_x003A_Name3_x002F_Number>4</_x002F_Name_x003A_Name3_x002F_Number>
			    <_x002F_Name_x003A_Name3_x002F_Weight>1.4</_x002F_Name_x003A_Name3_x002F_Weight>
			    <_x002F_Name_x003A_Name3_x002F_RowCount>1</_x002F_Name_x003A_Name3_x002F_RowCount>
			  </row>
			  <row>
			    <Site>Site3</Site>
			    <_x002F_Name_x003A_Name1_x002F_Unit>Unit1</_x002F_Name_x003A_Name1_x002F_Unit>
			    <_x002F_Name_x003A_Name1_x002F_Group>Group1</_x002F_Name_x003A_Name1_x002F_Group>
			    <_x002F_Name_x003A_Name1_x002F_Number>5</_x002F_Name_x003A_Name1_x002F_Number>
			    <_x002F_Name_x003A_Name1_x002F_Weight>2.1</_x002F_Name_x003A_Name1_x002F_Weight>
			    <_x002F_Name_x003A_Name1_x002F_RowCount>1</_x002F_Name_x003A_Name1_x002F_RowCount>
			    <_x002F_Name_x003A_Name3_x002F_Unit />
			    <_x002F_Name_x003A_Name3_x002F_Group />
			    <_x002F_Name_x003A_Name3_x002F_Number>0</_x002F_Name_x003A_Name3_x002F_Number>
			    <_x002F_Name_x003A_Name3_x002F_Weight>0</_x002F_Name_x003A_Name3_x002F_Weight>
			    <_x002F_Name_x003A_Name3_x002F_RowCount>0</_x002F_Name_x003A_Name3_x002F_RowCount>
			  </row>
			  <row>
			    <Site>Site5</Site>
			    <_x002F_Name_x003A_Name1_x002F_Unit>Unit6</_x002F_Name_x003A_Name1_x002F_Unit>
			    <_x002F_Name_x003A_Name1_x002F_Group>Group1</_x002F_Name_x003A_Name1_x002F_Group>
			    <_x002F_Name_x003A_Name1_x002F_Number>6</_x002F_Name_x003A_Name1_x002F_Number>
			    <_x002F_Name_x003A_Name1_x002F_Weight>5.1</_x002F_Name_x003A_Name1_x002F_Weight>
			    <_x002F_Name_x003A_Name1_x002F_RowCount>1</_x002F_Name_x003A_Name1_x002F_RowCount>
			    <_x002F_Name_x003A_Name3_x002F_Unit />
			    <_x002F_Name_x003A_Name3_x002F_Group />
			    <_x002F_Name_x003A_Name3_x002F_Number>0</_x002F_Name_x003A_Name3_x002F_Number>
			    <_x002F_Name_x003A_Name3_x002F_Weight>0</_x002F_Name_x003A_Name3_x002F_Weight>
			    <_x002F_Name_x003A_Name3_x002F_RowCount>0</_x002F_Name_x003A_Name3_x002F_RowCount>
			  </row>
			</DocumentElement>
			""";

		const string str_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName_json = """
			{
			  "rows": [
			    [
			      "Site1",
			      "Unit1",
			      "Group1, Group2",
			      3,
			      2.3,
			      2,
			      "Unit2",
			      "Group1",
			      4,
			      1.4,
			      1
			    ],
			    [
			      "Site3",
			      "Unit1",
			      "Group1",
			      5,
			      2.1,
			      1,
			      "",
			      "",
			      0,
			      0,
			      0
			    ],
			    [
			      "Site5",
			      "Unit6",
			      "Group1",
			      6,
			      5.1,
			      1,
			      "",
			      "",
			      0,
			      0,
			      0
			    ]
			  ]
			}
			""";

		const string str_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName_DictArr = """
			{
			  "rows": [
			    {
			      "Site": "Site1",
			      "/Name:Name1/Unit": "Unit1",
			      "/Name:Name1/Group": "Group1, Group2",
			      "/Name:Name1/Number": 3,
			      "/Name:Name1/Weight": 2.3,
			      "/Name:Name1/RowCount": 2,
			      "/Name:Name3/Unit": "Unit2",
			      "/Name:Name3/Group": "Group1",
			      "/Name:Name3/Number": 4,
			      "/Name:Name3/Weight": 1.4,
			      "/Name:Name3/RowCount": 1
			    },
			    {
			      "Site": "Site3",
			      "/Name:Name1/Unit": "Unit1",
			      "/Name:Name1/Group": "Group1",
			      "/Name:Name1/Number": 5,
			      "/Name:Name1/Weight": 2.1,
			      "/Name:Name1/RowCount": 1,
			      "/Name:Name3/Unit": "",
			      "/Name:Name3/Group": "",
			      "/Name:Name3/Number": 0,
			      "/Name:Name3/Weight": 0,
			      "/Name:Name3/RowCount": 0
			    },
			    {
			      "Site": "Site5",
			      "/Name:Name1/Unit": "Unit6",
			      "/Name:Name1/Group": "Group1",
			      "/Name:Name1/Number": 6,
			      "/Name:Name1/Weight": 5.1,
			      "/Name:Name1/RowCount": 1,
			      "/Name:Name3/Unit": "",
			      "/Name:Name3/Group": "",
			      "/Name:Name3/Number": 0,
			      "/Name:Name3/Weight": 0,
			      "/Name:Name3/RowCount": 0
			    }
			  ]
			}
			""";

		// this originally had dummy values for Name3 in the last 2 rows. Not sure what is more correct.
		const string nest_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName = """
			{
			  "rows": [
			    {
			      "Site": "Site1",
			      "NameList": [
			        {
			          "Name": "Name1",
			          "Unit": "Unit1",
			          "Group": "Group1, Group2",
			          "Number": 3,
			          "Weight": 2.3,
			          "RowCount": 2
			        },
			        {
			          "Name": "Name3",
			          "Unit": "Unit2",
			          "Group": "Group1",
			          "Number": 4,
			          "Weight": 1.4,
			          "RowCount": 1
			        }
			      ]
			    },
			    {
			      "Site": "Site3",
			      "NameList": [
			        {
			          "Name": "Name1",
			          "Unit": "Unit1",
			          "Group": "Group1",
			          "Number": 5,
			          "Weight": 2.1,
			          "RowCount": 1
			        }
			      ]
			    },
			    {
			      "Site": "Site5",
			      "NameList": [
			        {
			          "Name": "Name1",
			          "Unit": "Unit6",
			          "Group": "Group1",
			          "Number": 6,
			          "Weight": 5.1,
			          "RowCount": 1
			        }
			      ]
			    }
			  ]
			}
			""";

		[TestMethod]
		public void TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName()
		{
			Pivoter<Test1Row> p = GetPivoterTestData();

			var fields = p.Fields.ToDictionary(k => k.Name);
			fields[nameof(Test1Row.Site)].Area = Area.Row;
			fields[nameof(Test1Row.Site)].SortOrder = SortOrder.Asc;

			fields[nameof(Test1Row.Name)].Area = Area.Column;
			fields[nameof(Test1Row.Name)].SortOrder = SortOrder.Asc;

			var gdata_fis = p.GetGroupedData_FastIntersect();// createEmptyIntersects: true);
			var gdata_ptb = p.GetGroupedData_PivotTableBuilder();// createEmptyIntersects: true);

			var pres_fis = new Presentation<Test1Row>(gdata_fis);
			var pres_ptb = new Presentation2<Test1Row>(gdata_ptb);

			var dt_fis = pres_fis.GetDataTable(createEmptyIntersects: true);
			var dt_ptb = pres_ptb.GetDataTable(createEmptyIntersects: true);

			string xml_ptb = dt_ptb.ToXml();
			string xml_fis = dt_fis.ToXml();

			Assert.AreEqual(str_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName, xml_ptb);
			Assert.AreEqual(xml_ptb, xml_fis);

			string json_fis = ToJson(pres_fis.GetTable_Array(createEmptyIntersects: true));
			string json_ptb = ToJson(pres_ptb.GetTable_Array(createEmptyIntersects: true));
			Assert.AreEqual(str_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName_json, json_fis);
			Assert.AreEqual(json_fis, json_ptb);

			var tblDictArr_fis = pres_fis.GetTable_FlatKeyValueList_CompleteColumns(createEmptyIntersects: true);
			var json_tblDictArr_fis = ToJson(tblDictArr_fis);
			Assert.AreEqual(str_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName_DictArr, json_tblDictArr_fis);

			var nested_json_fis = ToJson(pres_fis.GetTable_NestedKeyValueList_VariableColumns());// createEmptyIntersects: true));
			Assert.AreEqual(nest_TestCompareFastAndSlow_RowGroupOnSite_ColGroupOnName, nested_json_fis);
		}

		const string str_TestCompareFastAndSlow_ColGroupOnName = """
			<DocumentElement>
			  <row>
			    <_x002F_Name_x003A_Name1_x002F_Site>Site1, Site3, Site5</_x002F_Name_x003A_Name1_x002F_Site>
			    <_x002F_Name_x003A_Name1_x002F_Unit>Unit1, Unit6</_x002F_Name_x003A_Name1_x002F_Unit>
			    <_x002F_Name_x003A_Name1_x002F_Group>Group1, Group2</_x002F_Name_x003A_Name1_x002F_Group>
			    <_x002F_Name_x003A_Name1_x002F_Number>14</_x002F_Name_x003A_Name1_x002F_Number>
			    <_x002F_Name_x003A_Name1_x002F_Weight>9.5</_x002F_Name_x003A_Name1_x002F_Weight>
			    <_x002F_Name_x003A_Name1_x002F_RowCount>4</_x002F_Name_x003A_Name1_x002F_RowCount>
			    <_x002F_Name_x003A_Name3_x002F_Site>Site1</_x002F_Name_x003A_Name3_x002F_Site>
			    <_x002F_Name_x003A_Name3_x002F_Unit>Unit2</_x002F_Name_x003A_Name3_x002F_Unit>
			    <_x002F_Name_x003A_Name3_x002F_Group>Group1</_x002F_Name_x003A_Name3_x002F_Group>
			    <_x002F_Name_x003A_Name3_x002F_Number>4</_x002F_Name_x003A_Name3_x002F_Number>
			    <_x002F_Name_x003A_Name3_x002F_Weight>1.4</_x002F_Name_x003A_Name3_x002F_Weight>
			    <_x002F_Name_x003A_Name3_x002F_RowCount>1</_x002F_Name_x003A_Name3_x002F_RowCount>
			  </row>
			</DocumentElement>
			""";

		const string str_TestCompareFastAndSlow_ColGroupOnName_json = """
			{
			  "rows": [
			    [
			      "Site1, Site3, Site5",
			      "Unit1, Unit6",
			      "Group1, Group2",
			      14,
			      9.5,
			      4,
			      "Site1",
			      "Unit2",
			      "Group1",
			      4,
			      1.4,
			      1
			    ]
			  ]
			}
			""";

		const string str_TestCompareFastAndSlow_ColGroupOnName_DictArr = """
			{
			  "rows": [
			    {
			      "/Name:Name1/Site": "Site1, Site3, Site5",
			      "/Name:Name1/Unit": "Unit1, Unit6",
			      "/Name:Name1/Group": "Group1, Group2",
			      "/Name:Name1/Number": 14,
			      "/Name:Name1/Weight": 9.5,
			      "/Name:Name1/RowCount": 4,
			      "/Name:Name3/Site": "Site1",
			      "/Name:Name3/Unit": "Unit2",
			      "/Name:Name3/Group": "Group1",
			      "/Name:Name3/Number": 4,
			      "/Name:Name3/Weight": 1.4,
			      "/Name:Name3/RowCount": 1
			    }
			  ]
			}
			""";

		const string nested_TestCompareFastAndSlow_ColGroupOnName = """
			{
			  "rows": [
			    {
			      "NameList": [
			        {
			          "Name": "Name1",
			          "Site": "Site1, Site3, Site5",
			          "Unit": "Unit1, Unit6",
			          "Group": "Group1, Group2",
			          "Number": 14,
			          "Weight": 9.5,
			          "RowCount": 4
			        },
			        {
			          "Name": "Name3",
			          "Site": "Site1",
			          "Unit": "Unit2",
			          "Group": "Group1",
			          "Number": 4,
			          "Weight": 1.4,
			          "RowCount": 1
			        }
			      ]
			    }
			  ]
			}
			""";

		[TestMethod]
	// Expected: when only group in col, 1 row in the result with only totalt
		public void TestCompareFastAndSlow_ColGroupOnName()
		{
			Pivoter<Test1Row> p = GetPivoterTestData();

			var fields = p.Fields.ToDictionary(k => k.Name);

			fields[nameof(Test1Row.Name)].Area = Area.Column;
			fields[nameof(Test1Row.Name)].SortOrder = SortOrder.Asc;

			var gdata_fis = p.GetGroupedData_FastIntersect();
			var gdata_ptb = p.GetGroupedData_PivotTableBuilder();

			var pres_fis = new Presentation<Test1Row>(gdata_fis);
			var pres_ptb = new Presentation2<Test1Row>(gdata_ptb);

			var dt_fis = pres_fis.GetDataTable();
			var dt_ptb = pres_ptb.GetDataTable();

			string xml_fis = dt_fis.ToXml();
			string xml_ptb = dt_ptb.ToXml();

			Assert.AreEqual(str_TestCompareFastAndSlow_ColGroupOnName, xml_ptb);
			Assert.AreEqual(xml_ptb, xml_fis);

			string json_fis = ToJson(pres_fis.GetTable_Array());
			string json_ptb = ToJson(pres_ptb.GetTable_Array());
			Assert.AreEqual(str_TestCompareFastAndSlow_ColGroupOnName_json, json_fis);
			Assert.AreEqual(json_fis, json_ptb);

			var tblDictArr_fis = pres_fis.GetTable_FlatKeyValueList_CompleteColumns();
			var json_tblDictArr_fis = ToJson(tblDictArr_fis);
			Assert.AreEqual(str_TestCompareFastAndSlow_ColGroupOnName_DictArr, json_tblDictArr_fis);
			var tblDictArr_ptb = pres_ptb.GetTable_FlatKeyValueList_CompleteColumns();
			var json_tblDictArr_ptb = ToJson(tblDictArr_ptb);
			Assert.AreEqual(json_tblDictArr_ptb, json_tblDictArr_fis);

			var nested_json_fis = ToJson(pres_fis.GetTable_NestedKeyValueList_VariableColumns());
			Assert.AreEqual(nested_TestCompareFastAndSlow_ColGroupOnName, nested_json_fis);

			// FIXME: ptb return no rows. It should return one total row?
			// What is right, what is wrong?
//			var nested_json_ptb = ToJson(pres_ptb.GetTable_NestedKeyValueList_VariableColumns());
//			Assert.AreEqual(nested_json_ptb, nested_json_fis);
		}

		const string str_TestCompareFastAndSlow_NoGroup = """
			<DocumentElement>
			  <row>
			    <Site>Site1, Site3, Site5</Site>
			    <Unit>Unit1, Unit2, Unit6</Unit>
			    <Group>Group1, Group2</Group>
			    <Name>Name1, Name3</Name>
			    <Number>18</Number>
			    <Weight>10.9</Weight>
			    <RowCount>5</RowCount>
			  </row>
			</DocumentElement>
			""";

		const string str_TestCompareFastAndSlow_NoGroup_json = """
			{
			  "rows": [
			    [
			      "Site1, Site3, Site5",
			      "Unit1, Unit2, Unit6",
			      "Group1, Group2",
			      "Name1, Name3",
			      18,
			      10.9,
			      5
			    ]
			  ]
			}
			""";

		const string str_TestCompareFastAndSlow_NoGroup_DictArr = """
			{
			  "rows": [
			    {
			      "Site": "Site1, Site3, Site5",
			      "Unit": "Unit1, Unit2, Unit6",
			      "Group": "Group1, Group2",
			      "Name": "Name1, Name3",
			      "Number": 18,
			      "Weight": 10.9,
			      "RowCount": 5
			    }
			  ]
			}
			""";

		const string nested_TestCompareFastAndSlow_NoGroup = """
			{
			  "rows": [
			    {
			      "Site": "Site1, Site3, Site5",
			      "Unit": "Unit1, Unit2, Unit6",
			      "Group": "Group1, Group2",
			      "Name": "Name1, Name3",
			      "Number": 18,
			      "Weight": 10.9,
			      "RowCount": 5
			    }
			  ]
			}
			""";

		[TestMethod]
		// Expected: 1 row with totals
		public void TestCompareFastAndSlow_NoGroup()
		{
			Pivoter<Test1Row> p = GetPivoterTestData();

			var fields = p.Fields.ToDictionary(k => k.Name);

			var gdata_fis = p.GetGroupedData_FastIntersect();
			var gdata_ptb = p.GetGroupedData_PivotTableBuilder();

			var pres_fis = new Presentation<Test1Row>(gdata_fis);
			var pres_ptb = new Presentation2<Test1Row>(gdata_ptb);

			var dt_fis = pres_fis.GetDataTable();
			var dt_ptb = pres_ptb.GetDataTable();

			string xml_ptb = dt_ptb.ToXml();
			string xml_fis = dt_fis.ToXml();

			Assert.AreEqual(str_TestCompareFastAndSlow_NoGroup, xml_ptb);
			Assert.AreEqual(xml_ptb, xml_fis);

			string json_fis = ToJson(pres_fis.GetTable_Array());
			string json_ptb = ToJson(pres_ptb.GetTable_Array());
			Assert.AreEqual(str_TestCompareFastAndSlow_NoGroup_json, json_fis);
			Assert.AreEqual(json_fis, json_ptb);

			var tblDictArr_fis = pres_fis.GetTable_FlatKeyValueList_CompleteColumns();
			var json_tblDictArr_fis = ToJson(tblDictArr_fis);
			Assert.AreEqual(str_TestCompareFastAndSlow_NoGroup_DictArr, json_tblDictArr_fis);

			// this produce same result as GetTable_FlatDict in this case (no col groups)
			var nested_json_fis = ToJson(pres_fis.GetTable_NestedKeyValueList_VariableColumns());
			Assert.AreEqual(nested_TestCompareFastAndSlow_NoGroup, nested_json_fis);
		}

		const string TestGroupSiteThenUnitSortBoth_nested = """
			{
			  "rows": [
			    {
			      "Site": "Site1",
			      "Unit": "Unit2",
			      "GroupList": [
			        {
			          "Group": "Group1",
			          "Name": "Name3",
			          "Number": 4,
			          "Weight": 1.4,
			          "RowCount": 1
			        }
			      ]
			    },
			    {
			      "Site": "Site1",
			      "Unit": "Unit1",
			      "GroupList": [
			        {
			          "Group": "Group2",
			          "Name": "Name1",
			          "Number": 2,
			          "Weight": 1.2,
			          "RowCount": 1
			        },
			        {
			          "Group": "Group1",
			          "Name": "Name1",
			          "Number": 1,
			          "Weight": 1.1,
			          "RowCount": 1
			        }
			      ]
			    },
			    {
			      "Site": "Site3",
			      "Unit": "Unit1",
			      "GroupList": [
			        {
			          "Group": "Group1",
			          "Name": "Name1",
			          "Number": 5,
			          "Weight": 2.1,
			          "RowCount": 1
			        }
			      ]
			    },
			    {
			      "Site": "Site5",
			      "Unit": "Unit6",
			      "GroupList": [
			        {
			          "Group": "Group1",
			          "Name": "Name1",
			          "Number": 6,
			          "Weight": 5.1,
			          "RowCount": 1
			        }
			      ]
			    }
			  ]
			}
			""";

		const string TestGroupSiteThenUnitSortBoth_flat = """
			{
			  "rows": [
			    {
			      "Site": "Site1",
			      "Unit": "Unit2",
			      "/Group:Group2/Name": null,
			      "/Group:Group2/Number": null,
			      "/Group:Group2/Weight": null,
			      "/Group:Group2/RowCount": null,
			      "/Group:Group1/Name": "Name3",
			      "/Group:Group1/Number": 4,
			      "/Group:Group1/Weight": 1.4,
			      "/Group:Group1/RowCount": 1
			    },
			    {
			      "Site": "Site1",
			      "Unit": "Unit1",
			      "/Group:Group2/Name": "Name1",
			      "/Group:Group2/Number": 2,
			      "/Group:Group2/Weight": 1.2,
			      "/Group:Group2/RowCount": 1,
			      "/Group:Group1/Name": "Name1",
			      "/Group:Group1/Number": 1,
			      "/Group:Group1/Weight": 1.1,
			      "/Group:Group1/RowCount": 1
			    },
			    {
			      "Site": "Site3",
			      "Unit": "Unit1",
			      "/Group:Group2/Name": null,
			      "/Group:Group2/Number": null,
			      "/Group:Group2/Weight": null,
			      "/Group:Group2/RowCount": null,
			      "/Group:Group1/Name": "Name1",
			      "/Group:Group1/Number": 5,
			      "/Group:Group1/Weight": 2.1,
			      "/Group:Group1/RowCount": 1
			    },
			    {
			      "Site": "Site5",
			      "Unit": "Unit6",
			      "/Group:Group2/Name": null,
			      "/Group:Group2/Number": null,
			      "/Group:Group2/Weight": null,
			      "/Group:Group2/RowCount": null,
			      "/Group:Group1/Name": "Name1",
			      "/Group:Group1/Number": 6,
			      "/Group:Group1/Weight": 5.1,
			      "/Group:Group1/RowCount": 1
			    }
			  ]
			}
			""";

		const string TestGroupSiteThenUnitSortBoth_xml_nest = """
			<?xml version="1.0" encoding="utf-8"?>
			<Table>
			  <Rows>
			    <Row>
			      <Site>Site1</Site>
			      <Unit>Unit2</Unit>
			      <GroupList>
			        <Entry>
			          <Group>Group1</Group>
			          <Name>Name3</Name>
			          <Number>4</Number>
			          <Weight>1.4</Weight>
			          <RowCount>1</RowCount>
			        </Entry>
			      </GroupList>
			    </Row>
			    <Row>
			      <Site>Site1</Site>
			      <Unit>Unit1</Unit>
			      <GroupList>
			        <Entry>
			          <Group>Group2</Group>
			          <Name>Name1</Name>
			          <Number>2</Number>
			          <Weight>1.2</Weight>
			          <RowCount>1</RowCount>
			        </Entry>
			        <Entry>
			          <Group>Group1</Group>
			          <Name>Name1</Name>
			          <Number>1</Number>
			          <Weight>1.1</Weight>
			          <RowCount>1</RowCount>
			        </Entry>
			      </GroupList>
			    </Row>
			    <Row>
			      <Site>Site3</Site>
			      <Unit>Unit1</Unit>
			      <GroupList>
			        <Entry>
			          <Group>Group1</Group>
			          <Name>Name1</Name>
			          <Number>5</Number>
			          <Weight>2.1</Weight>
			          <RowCount>1</RowCount>
			        </Entry>
			      </GroupList>
			    </Row>
			    <Row>
			      <Site>Site5</Site>
			      <Unit>Unit6</Unit>
			      <GroupList>
			        <Entry>
			          <Group>Group1</Group>
			          <Name>Name1</Name>
			          <Number>6</Number>
			          <Weight>5.1</Weight>
			          <RowCount>1</RowCount>
			        </Entry>
			      </GroupList>
			    </Row>
			  </Rows>
			</Table>
			""";

		const string TestGroupSiteThenUnitSortBoth_xml_flat = """
			<?xml version="1.0" encoding="utf-8"?>
			<Table>
			  <Rows>
			    <Row>
			      <Site>Site1</Site>
			      <Unit>Unit2</Unit>
			      </Group:Group2/Name />
			      </Group:Group2/Number />
			      </Group:Group2/Weight />
			      </Group:Group2/RowCount />
			      </Group:Group1/Name>Name3<//Group:Group1/Name>
			      </Group:Group1/Number>4<//Group:Group1/Number>
			      </Group:Group1/Weight>1.4<//Group:Group1/Weight>
			      </Group:Group1/RowCount>1<//Group:Group1/RowCount>
			    </Row>
			    <Row>
			      <Site>Site1</Site>
			      <Unit>Unit1</Unit>
			      </Group:Group2/Name>Name1<//Group:Group2/Name>
			      </Group:Group2/Number>2<//Group:Group2/Number>
			      </Group:Group2/Weight>1.2<//Group:Group2/Weight>
			      </Group:Group2/RowCount>1<//Group:Group2/RowCount>
			      </Group:Group1/Name>Name1<//Group:Group1/Name>
			      </Group:Group1/Number>1<//Group:Group1/Number>
			      </Group:Group1/Weight>1.1<//Group:Group1/Weight>
			      </Group:Group1/RowCount>1<//Group:Group1/RowCount>
			    </Row>
			    <Row>
			      <Site>Site3</Site>
			      <Unit>Unit1</Unit>
			      </Group:Group2/Name />
			      </Group:Group2/Number />
			      </Group:Group2/Weight />
			      </Group:Group2/RowCount />
			      </Group:Group1/Name>Name1<//Group:Group1/Name>
			      </Group:Group1/Number>5<//Group:Group1/Number>
			      </Group:Group1/Weight>2.1<//Group:Group1/Weight>
			      </Group:Group1/RowCount>1<//Group:Group1/RowCount>
			    </Row>
			    <Row>
			      <Site>Site5</Site>
			      <Unit>Unit6</Unit>
			      </Group:Group2/Name />
			      </Group:Group2/Number />
			      </Group:Group2/Weight />
			      </Group:Group2/RowCount />
			      </Group:Group1/Name>Name1<//Group:Group1/Name>
			      </Group:Group1/Number>6<//Group:Group1/Number>
			      </Group:Group1/Weight>5.1<//Group:Group1/Weight>
			      </Group:Group1/RowCount>1<//Group:Group1/RowCount>
			    </Row>
			  </Rows>
			</Table>
			""";

		[TestMethod]
		public void TestGroupSiteThenUnitSortBoth()
		{
			var td = GetPivoterTestData();
			var sf = td.Fields.Single(f => f.Name == "Site");
			sf.Area = Area.Row;
			sf.GroupIndex = 0;
			sf.SortOrder = SortOrder.Asc;
			var su = td.Fields.Single(f => f.Name == "Unit");
			su.Area = Area.Row;
			su.GroupIndex = 1;
			su.SortOrder = SortOrder.Desc;
			var sg = td.Fields.Single(f => f.Name == "Group");
			sg.Area = Area.Column;
			sg.GroupIndex = 0;
			sg.SortOrder = SortOrder.Desc;

			var gdata_ptb = td.GetGroupedData_PivotTableBuilder();
			var pres_ptb = new Presentation2<Test1Row>(gdata_ptb);
			var nested_tbl_ptb = pres_ptb.GetTable_NestedKeyValueList_VariableColumns();
			var json_ptb = ToJson(nested_tbl_ptb);
			Assert.AreEqual(TestGroupSiteThenUnitSortBoth_nested, json_ptb);

			var flat_tbl_ptb = pres_ptb.GetTable_FlatKeyValueList_CompleteColumns();
//			flat.Columns = null;
//			flat.ColumnGroups = null;
	//		flat.RowGroups = null;
			var flat_json_ptb = ToJson(flat_tbl_ptb);
			Assert.AreEqual(TestGroupSiteThenUnitSortBoth_flat, flat_json_ptb);

			var xml_nest_ptb = nested_tbl_ptb.ToXml();
			Assert.AreEqual(TestGroupSiteThenUnitSortBoth_xml_nest, xml_nest_ptb);

			var xml_flat_ptb = flat_tbl_ptb.ToXml();
			Assert.AreEqual(TestGroupSiteThenUnitSortBoth_xml_flat, xml_flat_ptb);

			// TODO: add fis tests!!
		}


		private static Pivoter<Test1Row> GetPivoterTestData()
		{
			var r1 = new Test1Row { Site = "Site1", Unit = "Unit1", Group = "Group1", Name = "Name1", Number = 1, Weight = 1.1 };
			var r2 = new Test1Row { Site = "Site1", Unit = "Unit1", Group = "Group2", Name = "Name1", Number = 2, Weight = 1.2 };
			var r3 = new Test1Row { Site = "Site3", Unit = "Unit1", Group = "Group1", Name = "Name1", Number = 5, Weight = 2.1 };
			var r4 = new Test1Row { Site = "Site1", Unit = "Unit2", Group = "Group1", Name = "Name3", Number = 4, Weight = 1.4 };
			var r5 = new Test1Row { Site = "Site5", Unit = "Unit6", Group = "Group1", Name = "Name1", Number = 6, Weight = 5.1 };
			var rows = new[] { r1, r2, r3, r4, r5 };

			var p1 = new Field<Test1Row, string>(nameof(Test1Row.Site), rows => Aggregators.CommaList(rows, r => r.Site));
			var p2 = new Field<Test1Row, string>(nameof(Test1Row.Unit), rows => Aggregators.CommaList(rows, r => r.Unit));
			var p3 = new Field<Test1Row, string>(nameof(Test1Row.Group), rows => Aggregators.CommaList(rows, r => r.Group));
			var p4 = new Field<Test1Row, string>(nameof(Test1Row.Name), rows => Aggregators.CommaList(rows, r => r.Name));
			var p5 = new Field<Test1Row, int>(nameof(Test1Row.Number), rows => rows.Sum(r => r.Number));
			var p6 = new Field<Test1Row, double>(nameof(Test1Row.Weight), rows => rows.Sum(r => r.Weight));
			var p7 = new Field<Test1Row, int>("RowCount", rows => rows.Count());
			var props = new Field[] { p1, p2, p3, p4, p5, p6, p7 };

			var p = new Pivoter<Test1Row>(rows, props);
			return p;
		}




	}
}