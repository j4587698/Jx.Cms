function enter_down(event) {
    if(event.keyCode == "13") {
        stopDefault(event);
    }
}

function stopDefault(e) {
    //如果提供了事件对象，则这是一个非IE浏览器   
    if(e && e.preventDefault) {
        //阻止默认浏览器动作(W3C)  
        e.preventDefault();
    } else {
        //IE中阻止函数器默认动作的方式   
        window.event.returnValue = false;
    }
    return false;
} 