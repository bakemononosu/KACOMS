function loadVideo(videoElm, videoPath) {
    //console.log(`==>loadVideo('${location.origin}${videoPath}')`);
    if (Hls.isSupported()) {
        const hls = new Hls();

        hls.on(Hls.Events.MEDIA_ATTACHED, function () {
            //console.log('video and hls.js are now bound togethrt !');
        });

        hls.on(Hls.Events.MEDIA_PARSED, function (event, data) {
            //console.log(`manifest loaded ${data.levels.length} qua`);
            video.play();
        });

        hls.loadSource(`${location.origin}${videoPath}`);
        hls.attachMedia(videoElm);
    }
}