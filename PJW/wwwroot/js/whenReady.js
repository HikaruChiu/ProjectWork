
/*
   * 傳遞函式給whenReady()
   * 當文件解析完畢且為操作準備就緒時，函式作為document的方法呼叫
   */
var whenReady = (function () {               //這個函式返回whenReady()函式
    var funcs = [];             //當獲得事件時，要執行的函式
    var ready = false;          //當觸發事件處理程式時,切換為true

    //當文件就緒時,呼叫事件處理程式
    function handler(e) {
        if (ready) return;       //確保事件處理程式只完整執行一次

        //如果發生onreadystatechange事件，但其狀態不是complete的話,那麼文件尚未準備好
        if (e.type === 'onreadystatechange' && document.readyState !== 'complete') {
            return;
        }

        //執行所有註冊函式
        //注意每次都要計算funcs.length
        //以防這些函式的呼叫可能會導致註冊更多的函式
        for (var i = 0; i < funcs.length; i++) {
            funcs[i].call(document);
        }
        //事件處理函式完整執行,切換ready狀態, 並移除所有函式
        ready = true;
        funcs = null;
    }
    //為接收到的任何事件註冊處理程式
    if (document.addEventListener) {
        document.addEventListener('DOMContentLoaded', handler, false);
        document.addEventListener('readystatechange', handler, false);            //IE9+
        window.addEventListener('load', handler, false);
    } else if (document.attachEvent) {
        document.attachEvent('onreadystatechange', handler);
        window.attachEvent('onload', handler);
    }
    //返回whenReady()函式
    return function whenReady(fn) {
        if (ready) { fn.call(document); }
        else { funcs.push(fn); }
    }
})();