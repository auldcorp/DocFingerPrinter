SystemD Unit Files for WebCrawler
=================================

To setup these unit files copy them to ```/usr/lib/systemd/system/``` and run the ```systemctl enable napkin_collector.timer && systemctl start napkin_collector.timer``` to start and enable the timer. By default the service will run every 15 min.
