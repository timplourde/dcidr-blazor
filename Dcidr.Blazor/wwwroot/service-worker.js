
(function () {

    // cache-then-network model
    // bump this any time the app changes
    const currentCacheName = "2";

    const loggingEnabled = true;

    function log(message) {
        if (loggingEnabled) {
            console.log('[SW] ' + message);
        }
    }

    function handleFetch(event) {
        const url = event.request.url;

        const ignoredHosts = ['localhost', 'www.google-analytics.com', 'www.googletagmanager.com'];
        const { hostname } = new URL(event.request.url);
        if (ignoredHosts.indexOf(hostname) >= 0) {
            return;
        }

        log('fetching resource: ' + url);

        event.respondWith(
            caches.match(event.request).then((r) => {
                if (r) {
                    log('cache hit ' + url);
                }
                return r || fetch(event.request).then((response) => {
                    log('cache miss ' + url)
                    return caches.open(currentCacheName).then((cache) => {
                        log('caching ' + url);
                        cache.put(event.request, response.clone());
                        return response;
                    });
                });
            })
        );
    }

    function cleanCache(event) {
        log('cleaning old caches');
        event.waitUntil(
            caches.keys().then((keyList) => {
                return Promise.all(keyList.map((key) => {
                    if (key !== currentCacheName) {
                        log('deleting cache ' + key);
                        return caches.delete(key);
                    }
                }));
            })
        );
    }

    self.addEventListener('fetch', handleFetch);
    self.addEventListener('activate', cleanCache);

})();

