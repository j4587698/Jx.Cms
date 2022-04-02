(function(a){if(typeof define!=="undefined"&&define.amd){define([],a)}else{if(typeof module!=="undefined"&&module.exports){module.exports=a()}else{window.scrollMonitor=a()}}})(function(){var c=function(){return window.pageYOffset||(document.documentElement&&document.documentElement.scrollTop)||document.body.scrollTop};var G={};var k=[];var E="visibilityChange";var B="enterViewport";var z="fullyEnterViewport";var o="exitViewport";var l="partiallyExitViewport";var w="locationChange";var n="stateChange";var p=[E,B,z,o,l,w,n];var F={top:0,bottom:0};var y=function(){return window.innerHeight||document.documentElement.clientHeight};var a=function(){return Math.max(document.body.scrollHeight,document.documentElement.scrollHeight,document.body.offsetHeight,document.documentElement.offsetHeight,document.documentElement.clientHeight)};G.viewportTop=null;G.viewportBottom=null;G.documentHeight=null;G.viewportHeight=y();var v;var s;var b;function t(){G.viewportTop=c();G.viewportBottom=G.viewportTop+G.viewportHeight;G.documentHeight=a();if(G.documentHeight!==v){b=k.length;while(b--){k[b].recalculateLocation()}v=G.documentHeight}}function r(){G.viewportHeight=y();t();q()}var d;function u(){clearTimeout(d);d=setTimeout(r,100)}var h;function q(){h=k.length;while(h--){k[h].update()}h=k.length;while(h--){k[h].triggerCallbacks()}}function m(P,I){var S=this;this.watchItem=P;if(!I){this.offsets=F}else{if(I===+I){this.offsets={top:I,bottom:I}}else{this.offsets={top:I.top||F.top,bottom:I.bottom||F.bottom}}}this.callbacks={};for(var N=0,M=p.length;N<M;N++){S.callbacks[p[N]]=[]}this.locked=false;var L;var Q;var R;var O;var H;var e;function K(i){if(i.length===0){return}H=i.length;while(H--){e=i[H];e.callback.call(S,s);if(e.isOne){i.splice(H,1)}}}this.triggerCallbacks=function J(){if(this.isInViewport&&!L){K(this.callbacks[B])}if(this.isFullyInViewport&&!Q){K(this.callbacks[z])}if(this.isAboveViewport!==R&&this.isBelowViewport!==O){K(this.callbacks[E]);if(!Q&&!this.isFullyInViewport){K(this.callbacks[z]);K(this.callbacks[l])}if(!L&&!this.isInViewport){K(this.callbacks[B]);K(this.callbacks[o])}}if(!this.isFullyInViewport&&Q){K(this.callbacks[l])}if(!this.isInViewport&&L){K(this.callbacks[o])}if(this.isInViewport!==L){K(this.callbacks[E])}switch(true){case L!==this.isInViewport:case Q!==this.isFullyInViewport:case R!==this.isAboveViewport:case O!==this.isBelowViewport:K(this.callbacks[n])}L=this.isInViewport;Q=this.isFullyInViewport;R=this.isAboveViewport;O=this.isBelowViewport};this.recalculateLocation=function(){if(this.locked){return}var U=this.top;var T=this.bottom;if(this.watchItem.nodeName){var j=this.watchItem.style.display;if(j==="none"){this.watchItem.style.display=""}var i=this.watchItem.getBoundingClientRect();this.top=i.top+G.viewportTop;this.bottom=i.bottom+G.viewportTop;if(j==="none"){this.watchItem.style.display=j}}else{if(this.watchItem===+this.watchItem){if(this.watchItem>0){this.top=this.bottom=this.watchItem}else{this.top=this.bottom=G.documentHeight-this.watchItem}}else{this.top=this.watchItem.top;this.bottom=this.watchItem.bottom}}this.top-=this.offsets.top;this.bottom+=this.offsets.bottom;this.height=this.bottom-this.top;if((U!==undefined||T!==undefined)&&(this.top!==U||this.bottom!==T)){K(this.callbacks[w])}};this.recalculateLocation();this.update();L=this.isInViewport;Q=this.isFullyInViewport;R=this.isAboveViewport;O=this.isBelowViewport}m.prototype={on:function(e,j,i){switch(true){case e===E&&!this.isInViewport&&this.isAboveViewport:case e===B&&this.isInViewport:case e===z&&this.isFullyInViewport:case e===o&&this.isAboveViewport&&!this.isInViewport:case e===l&&this.isAboveViewport:j.call(this,s);if(i){return}}if(this.callbacks[e]){this.callbacks[e].push({callback:j,isOne:i||false})}else{throw new Error("Tried to add a scroll monitor listener of type "+e+". Your options are: "+p.join(", "))}},off:function(H,I){if(this.callbacks[H]){for(var e=0,j;j=this.callbacks[H][e];e++){if(j.callback===I){this.callbacks[H].splice(e,1);break}}}else{throw new Error("Tried to remove a scroll monitor listener of type "+H+". Your options are: "+p.join(", "))}},one:function(e,i){this.on(e,i,true)},recalculateSize:function(){this.height=this.watchItem.offsetHeight+this.offsets.top+this.offsets.bottom;this.bottom=this.top+this.height},update:function(){this.isAboveViewport=this.top<G.viewportTop;this.isBelowViewport=this.bottom>G.viewportBottom;this.isInViewport=(this.top<=G.viewportBottom&&this.bottom>=G.viewportTop);this.isFullyInViewport=(this.top>=G.viewportTop&&this.bottom<=G.viewportBottom)||(this.isAboveViewport&&this.isBelowViewport)},destroy:function(){var I=k.indexOf(this),e=this;k.splice(I,1);for(var J=0,H=p.length;J<H;J++){e.callbacks[p[J]].length=0}},lock:function(){this.locked=true},unlock:function(){this.locked=false}};var g=function(e){return function(j,i){this.on.call(this,e,j,i)}};for(var C=0,A=p.length;C<A;C++){var f=p[C];m.prototype[f]=g(f)}try{t()}catch(D){try{window.$(t)}catch(D){throw new Error("If you must put scrollMonitor in the <head>, you must use jQuery.")
}}function x(e){s=e;t();q()}if(window.addEventListener){window.addEventListener("scroll",x);window.addEventListener("resize",u)}else{window.attachEvent("onscroll",x);window.attachEvent("onresize",u)}G.beget=G.create=function(i,j){if(typeof i==="string"){i=document.querySelector(i)}else{if(i&&i.length>0){i=i[0]}}var e=new m(i,j);k.push(e);e.update();return e};G.update=function(){s=null;t();q()};G.recalculateLocations=function(){G.documentHeight=0;G.update()};return G});

// �˵�
$(document).ready(function() {
    var $account = $('#top-header');
    var $header = $('#menu-box, #main-search, #mobile-nav');
    var $minisb = $('#content');
    var $footer = $('#footer');

    var accountWatcher = scrollMonitor.create($account);
    var headerWatcher = scrollMonitor.create($header);

    var footerWatcherTop = $minisb.height() + $header.height();
    var footerWatcher = scrollMonitor.create($footer, {
        top: footerWatcherTop
    });

    accountWatcher.lock();
    headerWatcher.lock();

    accountWatcher.visibilityChange(function() {
        $header.toggleClass('shadow', !accountWatcher.isInViewport);
    });
    headerWatcher.visibilityChange(function() {
        $minisb.toggleClass('shadow', !headerWatcher.isInViewport);
    });

    footerWatcher.fullyEnterViewport(function() {
        if (footerWatcher.isAboveViewport) {
            $minisb.removeClass('shadow').addClass('hug-footer')
        }
    });
    footerWatcher.partiallyExitViewport(function() {
        if (!footerWatcher.isAboveViewport) {
            $minisb.addClass('fixed').removeClass('hug-footer')
        }
    });
})

// ����
$(document).ready(function() {
    var $account = $('#sidebar');
    var $header = $('.sidebar-roll');
    var $minisb = $('#content');
    var $footer = $('#footer');

    var accountWatcher = scrollMonitor.create($account);
    var headerWatcher = scrollMonitor.create($header);

    var footerWatcherTop = $minisb.height() + $header.height();

    accountWatcher.lock();
    headerWatcher.lock();

    accountWatcher.visibilityChange(function() {
        $header.toggleClass('follow', !accountWatcher.isInViewport);
    });
})

// �����б�
$(document).ready(function(a) {
    if (typeof scrollMonitor != 'undefined') {
        a(".thumbnail, .content-image img, .single-content p img, #related-img img").each(function(i, el) {
            var ael = a(el),
            watcher = scrollMonitor.create(el, -100);
            ael.addClass('box-hide');
            watcher.enterViewport(function(ev) {
                if (!ael.hasClass('box-show')) {
                    ael.addClass('box-show');
                }
            });
        });
    }
})