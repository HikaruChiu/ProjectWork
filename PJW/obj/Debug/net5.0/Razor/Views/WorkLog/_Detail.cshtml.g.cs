#pragma checksum "D:\MyProject\PJW\PJW\Views\WorkLog\_Detail.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "577653a466511084da5e48fc529348e0fae241bd"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_WorkLog__Detail), @"mvc.1.0.view", @"/Views/WorkLog/_Detail.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\MyProject\PJW\PJW\Views\_ViewImports.cshtml"
using PJW.Web;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"577653a466511084da5e48fc529348e0fae241bd", @"/Views/WorkLog/_Detail.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a1b9716aa90c7cef9b5e537ef4f2378438930e99", @"/Views/_ViewImports.cshtml")]
    public class Views_WorkLog__Detail : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<!--檢視、送審、執行-->
<style>
    .WdatePickerDate {
        height: 35px;
        width: 90px;
        margin-left: 5px;
        margin-top: -32px;
        border-radius: 6px;
        border: 1px #cccccc solid;
        outline: none
    }
    .WdatePickerTime {
        height: 35px;
        width: 70px;
        margin-left: 5px;
        margin-top: -32px;
        border-radius: 6px;
        border: 1px #cccccc solid;
        outline: none
    }
</style>
<div class=""modal inmodal"" id=""WorkLogDetailModal"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <div class=""modal-title"">
                    任務狀態：<span id=""spanWorkLogStatus""></span>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <span id=""spanWorkLogScore""></span>
                </div>
            </div>
            <div class=""modal-body"">
                <div class=""form-group"">
        ");
            WriteLiteral("            <label class=\"control-label\">任務編號：</label>\r\n                    <span id=\"spanWorkLogNo\"></span>\r\n                    <input type=\"text\" id=\"workLogNo\"");
            BeginWriteAttribute("value", " value=\"", 1187, "\"", 1195, 0);
            EndWriteAttribute();
            WriteLiteral(@" readonly style=""display:none"" />
                </div>
                <div class=""form-group"">
                    <label class=""form-label"">任務分類：</label>
                    <span id=""spanWorkLogSort""></span>
                </div>
                <div class=""form-group"">
                    <label class=""form-label"">任務名稱：</label>
                    <span id=""spanWorkLogSubject""></span>
                </div>
                <div class=""form-group"">
                    <label class=""form-label"">任務說明：</label>
                    <span id=""spanWorkLogContent""></span>
                </div>
                <div class=""form-group"">
                    <label class=""form-label"">需求等級：</label>
                    <span id=""spanWorkLogPriority""></span>
                </div>
                <div class=""form-group"">
                    <label class=""form-label"">新增時間：</label>
                    <span id=""spanWorkLogAddDate""></span>
                </div>
                <div class=""form-grou");
            WriteLiteral(@"p"">
                    <label class=""form-label"">附件明細：</label>
                    <table role=""presentation"" class=""table table-striped"" id=""presentationDetail"" style=""font-size:12px"">
                        <tbody class=""files""></tbody>
                    </table>
                </div>
                <div class=""form-group"" id=""divWorkLogDept"" style=""display:none;"">
                    <label class=""form-label"">需求單位：</label>
                    <span id=""spanWorkLogDeptName""></span>
                </div>
                <div class=""form-group"" id=""divWorkLogNeedDate"" style=""display:none;"">
                    <label class=""form-label"">所需時間：</label>
                    <input type=""text"" id=""workStartDate"" class=""WdatePickerDate"" placeholder=""開始日期""
                           onclick=""WdatePicker({ dateFmt: 'yyyy-MM-dd', minDate: '%y-%M-{%d}', maxDate: '#F{$dp.$D(\'workEndDate\')}', readOnly:true })"">
                    <input type=""text"" id=""workStartTime"" class=""WdatePickerTime"" placeho");
            WriteLiteral(@"lder=""開始時間""
                           onclick=""WdatePicker({dateFmt:'HH:mm', readOnly:true})"">
                    至
                    <input type=""text"" id=""workEndDate"" class=""WdatePickerDate"" placeholder=""結束日期""
                           onclick=""WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'workStartDate\')}', readOnly:true})"">
                    <input type=""text"" id=""workEndTime"" class=""WdatePickerTime"" placeholder=""結束時間""
                           onclick=""WdatePicker({dateFmt:'HH:mm', readOnly:true})"">
                </div>
                <div class=""form-group"" id=""divWorkLogAssignDept"" style=""display:;"">
                    <label class=""form-label"">執行單位：</label>
                    <input class=""js-example-data-array-selected text-box"" id=""workLogAssignDeptId"" style=""width: 200px;"" />
                </div>
                <div class=""form-group"" id=""divWorkLogAssign"" style=""display:none;"">
                    <label class=""form-label"">執行人員：</label>
                    ");
            WriteLiteral(@"<input class=""js-example-data-array-selected text-box"" id=""workAssignId"" style=""width: 200px;"" />
                </div>
                <div class=""form-group"" id=""divWorkLogReply"" style=""display:none;"">
                    <label class=""form-label"">執行說明：</label>
                    <span id=""workLogReplyNoteMsg"" style=""display:none;color:red""></span>
                    <textarea class=""form-control"" id=""workLogReplyNote"" rows=""5"" onkeyup=""words_deal(this);""></textarea>
                </div>
                <div class=""form-group"" id=""divWorkLogReplyFile"" style=""display:none;"">
                    <span class=""btn btn-success fileinput-button"">
                        <span>附件上傳</span>
                        <!-- The file input field used as target for the file upload widget -->
                        <input id=""workLogReplyFile"" type=""file"" name=""workLogReplyFile"" multiple>
                    </span>

                    <table role=""presentation"" class=""table table-striped"" id=""presentat");
            WriteLiteral(@"ionReply"" style=""font-size:12px"">
                        <tbody class=""files""></tbody>
                    </table>
                    <div id=""progressReply"" class=""progress"">
                        <div class=""progress-bar progress-bar-info""></div>
                    </div>
                </div>
                <div class=""form-group"" id=""divWorkLogReplyList"" style=""display:none;"">

                    <label class=""form-label"">回覆內容：</label>
                    <table class=""table table-striped"" style=""font-size:12px"" id=""WorkLogReplyList"">
                    </table>
                </div>
                <div class=""form-group"" id=""divCaseClose"" style=""display:none;text-align:right"">

                    <label class=""form-label"">評鑑：</label>
                    <input class=""js-example-data-array-selected text-box"" id=""workLogScore"" style=""width: 100px;"" />
                </div>
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class");
            WriteLiteral(@"=""btn btn-success"" id=""assign-model"" style=""display:none;"">送出給執行人員</button>
                <button type=""button"" class=""btn btn-success"" id=""reply-model"" style=""display:none;"">執行說明回覆</button>
                <button type=""button"" class=""btn btn-success"" id=""check-model"" style=""display:none;"">送審</button>
                <button type=""button"" class=""btn btn-info"" id=""reject-model"" style=""display:none;"">退回</button>
                <button type=""button"" class=""btn btn-primary"" id=""caseclose-model"" style=""display:none;"">結案</button>
                <button type=""button"" class=""btn btn-success"" id=""share-model"" style=""display:none;"">分享出去</button>
                <button type=""button"" class=""btn btn-danger"" id=""close-model"">關閉</button>
            </div>
        </div>
    </div>
</div>
");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
