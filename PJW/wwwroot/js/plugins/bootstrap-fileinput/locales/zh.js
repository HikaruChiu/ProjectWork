/*!
 * FileInput Chinese Translations
 *
 * This file must be loaded after 'fileinput.js'. Patterns in braces '{}', or
 * any HTML markup tags in the messages must not be converted or translated.
 *
 * @see http://github.com/kartik-v/bootstrap-fileinput
 * @author kangqf <kangqingfei@gmail.com>
 *
 * NOTE: this file must be saved in UTF-8 encoding.
 */
(function (root, factory) {
    if (typeof root.define === 'function' && root.define.amd) {
        root.define(['jquery'], factory);
    } else factory(jQuery);
}(window, function ($) {
    "use strict";
    $.fn.fileinputLocales = $.fn.fileinputLocales || {};
    $.fn.fileinputLocales['zh'] = {
        fileSingle: '檔案',
        filePlural: '多個檔案',
        browseLabel: '選擇 &hellip;',
        removeLabel: '移除',
        removeTitle: '清除選中檔案',
        cancelLabel: '取消',
        cancelTitle: '取消進行中的上傳',
        uploadLabel: '上傳',
        uploadTitle: '上傳選中檔案',
        msgNo: '沒有',
        msgNoFilesSelected: '',
        msgCancelled: '取消',
        msgZoomModalHeading: '詳細預覽',
        msgSizeTooLarge: '檔案 "{name}" (<b>{size} KB</b>) 超過了允許大小 <b>{maxSize} KB</b>.',
        msgFilesTooLess: '你必須選擇最少 <b>{n}</b> {files} 來上傳. ',
        msgFilesTooMany: '選擇的上傳檔案個數 <b>({n})</b> 超出最大檔案的限制個數 <b>{m}</b>.',
        msgFileNotFound: '檔案 "{name}" 未找到!',
        msgFileSecured: '安全限制，爲了防止讀取檔案 "{name}".',
        msgFileNotReadable: '檔案 "{name}" 不可讀.',
        msgFilePreviewAborted: '取消 "{name}" 的預覽.',
        msgFilePreviewError: '讀取 "{name}" 時出現了一個錯誤.',
        msgInvalidFileType: '不正確的型別 "{name}". 只支援 "{types}" 型別的檔案.',
        msgInvalidFileExtension: '不正確的副檔名 "{name}". 只支援 "{extensions}" 的副檔名.',
        msgUploadAborted: '該檔案上傳被中止',
        msgUploadThreshold: 'Processing...',
        msgValidationError: '驗證錯誤',
        msgLoading: '載入第 {index} 檔案 共 {files} &hellip;',
        msgProgress: '載入第 {index} 檔案 共 {files} - {name} - {percent}% 完成.',
        msgSelected: '{n} {files} 選中',
        msgFoldersNotAllowed: '只支援拖拽檔案! 跳過 {n} 拖拽的資料夾.',
        msgImageWidthSmall: '寬度的影象檔案的"{name}"的必須是至少{size}畫素.',
        msgImageHeightSmall: '影象檔案的"{name}"的高度必須至少為{size}畫素.',
        msgImageWidthLarge: '寬度的影象檔案"{name}"不能超過{size}畫素.',
        msgImageHeightLarge: '影象檔案"{name}"的高度不能超過{size}畫素.',
        msgImageResizeError: '無法獲取的影象尺寸調整。',
        msgImageResizeException: '錯誤而調整影象大小。<pre>{errors}</pre>',
        dropZoneTitle: '拖拽檔案到這裡 &hellip;',
        dropZoneClickTitle: '<br>(or click to select {files})',
        fileActionSettings: {
            removeTitle: '刪除檔案',
            uploadTitle: '上傳檔案',
            zoomTitle: '檢視詳情',
            dragTitle: 'Move / Rearrange',
            indicatorNewTitle: '沒有上傳',
            indicatorSuccessTitle: '上傳',
            indicatorErrorTitle: '上傳錯誤',
            indicatorLoadingTitle: '上傳 ...'
        },
        previewZoomButtonTitles: {
            prev: 'View previous file',
            next: 'View next file',
            toggleheader: 'Toggle header',
            fullscreen: 'Toggle full screen',
            borderless: 'Toggle borderless mode',
            close: 'Close detailed preview'
        }
    };
}));