using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModel;
using Infrastructure.Excel;

namespace ServicesModel
{

    //如果要導出excel的第一列：標題是對應的中文 可以增加 ExcelClass 標籤 (中文名稱,對應的dbmodel的屬性名稱，排序值[越小越在前面])
    {% for field in ModelFields %}
    [ExcelClass("{{field.Comment}}", Column = "{{field.Name}}", OrderRule = {{forloop.index}})]
    {% endfor %}
    public class {{ModelClassName}}SM : {{ModelClassName}}
    {
        
        //也可以增加自定義列 並打上 ExcelField 標籤 (中文名稱,排序值)
        //[ExcelField("中文名稱", OrderRule = 10)]
        //public string Other { get; set; }

    }

   
}
