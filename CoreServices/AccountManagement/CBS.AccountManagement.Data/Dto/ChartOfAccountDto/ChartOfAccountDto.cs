using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Style.Fill;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.AccountManagement.Data
{

    public class ChartOfAccountDto
    {
        public string? Id { get; set; }
        public string AccountNumber { get; set; } //37123

 
        public string LabelFr { get; set; }
        public string LabelEn { get; set; }="";
        // Flags if this is a balance sheet account
        public bool IsBalanceAccount { get; set; }
        // Specifies if account balance can go negative
        public string? ParentAccountNumber { get; set; }//3712
        public string? ParentAccountId { get; set; }//3712
        public DateTime MigrationDate { get; set; }
        public bool IsDebit { get; set; }
 
        public string AccountCartegoryId { get; set; }
        public string AccountNumberNetwork { get; set; }
        public string AccountNumberCU { get; set; }

        public static List<JsData> ConvertToTreeNodeList(List<ChartOfAccountDto> chartOfAccounts, string language="0")
            {
                var treeNodes = new List<JsData>();
                List<JsData> AccountTreeNodes = new List<JsData>();
                foreach (var account in chartOfAccounts)
                {
                    var treeNode = new JsData
                    {
                        id = account.AccountNumber ?? "", // Assuming ParentAccountId is used for the id
                        text = /*$"{account.AccountNumber} - {account.LabelFr}",//:*/ $"{account.AccountNumber} - {account.LabelEn}",
                         children= new List<JsData>(),
                        state = new State { opened = false, disabled = false, selected = false },
                        icon = "mdi mdi-folder-outline",
                        parentId = account.ParentAccountNumber ?? "",
             
                        li_attr = new Dictionary<string, object>
                        {
                            {"class", "custom-node" },
                            {"data-bs-toggle","modal"},
                          {"data-bs-target","#largeModal"}

                        },
                        a_attr = new Dictionary<string, object>
                        {
                             {"class", "parent"},
                             {"Id", "parent_"+account.AccountNumber}
                        }
                    };
          
                treeNodes.Add(treeNode);
                }

                var reModel = JsData.BuildTree(treeNodes);
                
                return reModel;
            }
       
    }
}
