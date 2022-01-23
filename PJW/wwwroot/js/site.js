// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function WorkLogDetailInit() {

    $('#spanWorkLogScore').html('');
    $('#spanWorkLogStatus').html('');

    $('#presentationDetail > .files').html('');
    $("#divWorkLogNeedDate").hide();
    $("#divWorkLogAssignDept").hide();
    $("#workLogAssignDeptId").val('');
    $("#divWorkLogAssign").hide();
    $("#divWorkLogReply").hide();
    $("#divWorkLogReplyFile").hide();
    $('#presentationReply > .files').html('');

    $("#divWorkLogReplyList #WorkLogReplyList").html('');
    $("#divWorkLogReplyList").hide();

    
    $("#workLogScore").val('');
    $("#divCaseClose").hide();
    
    $("#assign-model").hide();
    $("#reply-model").hide();
    $("#check-model").hide();
    $("#reject-model").hide();
    $("#caseclose-model").hide();
    $("#share-model").hide();
    

}

function WorkLogDetail(workLogNo) {
    WorkLogDetailInit();

    //取得任務詳細內容
    QQT.ajax('/WorkLog/WorkLogDetail', 'Post', { workLogNo: workLogNo })
        .done(function (response) {
            if (!response || !response.Data) return;
            var data = response.Data[0];

            var formatWorkLogAddDate = dayjs(data.WorkLogAddDate).format('YYYY-MM-DD HH:mm:ss');
            $('#workLogNo').val(data.WorkLogNo);
            $('#spanWorkLogNo').text(data.WorkLogNo);
            $('#spanWorkLogStatus').html(data.WorkLogStatusName);
            $('#spanWorkLogSort').text(data.WorkLogSortName);
            $('#spanWorkLogSubject').html(data.WorkLogSubject);
            $('#spanWorkLogContent').html(data.WorkLogContent);
            $('#spanWorkLogPriority').html(data.WorkLogPriorityName);
            $('#spanWorkLogAddDate').text(formatWorkLogAddDate);
            if (data.WorkLogScore && data.WorkLogScore != '') {
                $('#spanWorkLogScore').html('任務評鑑' + data.WorkLogScore + '分');
            }

            $('#WorkLogDetailModal').modal({ backdrop: 'static', keyboard: false });
            $('#WorkLogDetailModal').modal('show');
        });
    //取得此任務的附件
    QQT.ajax('/WorkLog/GetWorkLogFile', 'Post', { workLogNo: workLogNo })
        .done(function (response) {
            if (!response || !response.Data) return;

            $.each(response.Data,
                function (index, item) {
                    $('#presentationDetail > .files').append(
                        $(
                            '<tr class="template-upload fade in" fileNo="' + item.work_log_file_no + '">' +
                            /*'<td><span class="preview"><img src="' + e.target.result + '" class="previewImg" /></span></td>' +*/
                            '<td style="color:blue">' + item.work_log_file_name + '</td>' +
                            '<td style="text-align:right"><button class="btn btn-primary submit" onclick="DownloadWorkLogFile(' + item.work_log_file_no + ')" style="background-color:#5cb85c;border-color:#4cae4c"><span>下載</span></button></td>' +
                            '</tr>'
                        )
                    );
                });

        });

    //取得此任務回覆的紀錄
    QQT.ajax('/WorkLog/GetWorkLogReplyList', 'Post', { workLogNo: workLogNo })
        .done(function (response) {
            if (!response || !response.Rows) return;
            
            if (response.Total > 0) {
                $("#divWorkLogReplyList").show();

                $.each(response.Rows,
                    function (index, item) {
                        $('#divWorkLogReplyList #WorkLogReplyList').append(
                            $(
                                '<thead style="background-color: #444;color: white;">' +
                                '<tr>' +
                                '<td>' + item.DeptName + ' ' + item.EngName + '</td>' +
                                '<td style="text-align:right">' + dayjs(item.work_log_reply_adddate).format('YYYY-MM-DD HH:mm:ss') + '</td>' +
                                '</tr>' +
                                '</thead>' +
                                '</tbody>' +
                                '<tr>' +
                                '<td colspan="2" style="color:red; font-size:14px;">' + item.work_log_reply_note + '</td>' +
                                '</tr>' +
                                '</tbody>'
                                
                            )
                        );

                        //GetWorkLogReplyFileList
                        //取得此任務回覆的附件
                        QQT.ajax('/WorkLog/GetWorkLogReplyFileList', 'Post', { workLogReplyNo: item.work_log_reply_no })
                            .done(function (response) {
                                if (!response || !response.Rows) return;
                                if (response.Total > 0) {
                                    $('#divWorkLogReplyList #WorkLogReplyList').append('<tfoot>');
                                }

                                $.each(response.Rows,
                                    function (index, item) {
                                        $('#divWorkLogReplyList #WorkLogReplyList').append(
                                            $(
                                                '<tr class="template-upload fade in" fileNo="' + item.work_log_reply_file_no + '">' +                                                
                                                '<td style="color:blue">' + item.work_log_reply_file_name + '</td>' +
                                                '<td style="text-align:right"><button class="btn btn-info" onclick="DownloadWorkLogReplyFile(' + item.work_log_reply_file_no + ')"><span>下載</span></button></td>' +
                                                '</tr>'
                                            )
                                        );
                                    });

                                if (response.Total > 0) {
                                    $('#divWorkLogReplyList #WorkLogReplyList').append('</tfoot>');
                                }
                            });
                    });
            }

        });
}

function DownloadWorkLogFile(fileNo) {
    filePath = '/WorkLog/DownloadFile/?workLogFileNo=' + fileNo;
    window.location.target = "_blank";
    window.location = filePath;

}

function DownloadWorkLogReplyFile(fileNo) {
    filePath = '/WorkLog/DownloadReplyFile/?workLogReplyFileNo=' + fileNo;
    window.location.target = "_blank";
    window.location = filePath;

}
