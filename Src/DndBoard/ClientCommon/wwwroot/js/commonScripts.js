﻿
function getElementOffsets(el) {
    var elRect = el.getBoundingClientRect();
    var obj = {};
    obj.offsetLeft = Math.floor(elRect.left);
    obj.offsetTop = Math.floor(elRect.top);
    return JSON.stringify(obj);
}


function createFileURL(fileContent) {
    var blob = new Blob([base64ToArrayBuffer(fileContent)], { type: 'image/png' });
    var url = URL.createObjectURL(blob);
    return url;
}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}


var iconsInstancesComponentInstance;
function initIconsInstancesComponent(instance) {
    iconsInstancesComponentInstance = instance;
    window.requestAnimationFrame(redraw);
}
var iconsModelsComponentInstance;
function initIconsModelsComponent(instance) {
    iconsModelsComponentInstance = instance;
    window.requestAnimationFrame(redraw);
}
function redraw() {
    iconsInstancesComponentInstance.invokeMethodAsync('Redraw');
    iconsModelsComponentInstance.invokeMethodAsync('Redraw');
    window.requestAnimationFrame(redraw);
}

function redrawAllImages(divCanvasId, imgList) {
    clearMapCanvas(divCanvasId);
    for (var i = 0; i < imgList.length; i++) {
        redrawImage(divCanvasId, imgList[i].ref, imgList[i].x, imgList[i].y);
    }
}
function clearMapCanvas(divCanvasId) {
    var mapCanvas = document.getElementById(divCanvasId).getElementsByTagName('canvas')[0];
    var canvasW = mapCanvas.getBoundingClientRect().width;
    var canvasH = mapCanvas.getBoundingClientRect().height;

    var ctx = mapCanvas.getContext('2d');
    ctx.clearRect(0, 0, canvasW, canvasH);
    ctx.fillStyle = 'rgb(0,200,0)';
    ctx.fillRect(0, 0, canvasW, canvasH);
}
function redrawImage(divCanvasId, img, x, y) {
    var mapCanvas = document.getElementById(divCanvasId).getElementsByTagName('canvas')[0];;
    var ctx = mapCanvas.getContext('2d');
    ctx.drawImage(img, x, y);
}
