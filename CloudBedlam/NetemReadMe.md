Examples of supported Network Emulation JSON configuration:

<b>Bandwidth</b>
<pre><code>
 "NetworkEmulation": {
      "Duration": 15,
      "EmulationType": "Bandwidth",
      "UpstreamSpeed" : 56,
      "DownstreamSpeed" : 33.6,
      "RunOrder": 0,
      "TargetEndpoints": {
         "Endpoints": [
           {
             "Port": 443,
             "Hostname": "www.bing.com",
             "Protocol": "ALL"
           },
           {
             "Port": 80,
             "Hostname": "www.msn.com",
             "Protocol": "ALL"
           },
           {
             "Port": 443,
             "Hostname": "www.google.com",
             "Protocol": "ALL"
           }
         ]
      }
  }
</code></pre>
   

<b>Corruption</b>
<pre><code>
 "NetworkEmulation": {
      "Duration": 15,
      "EmulationType": "Corruption",
      "PacketPercentage" : 0.10,
      "RunOrder": 0,
      "TargetEndpoints": {
         "Endpoints": [
           {
             "Port": 443,
             "Hostname": "www.bing.com",
             "Protocol": "ALL"
           },
           {
             "Port": 80,
             "Hostname": "www.msn.com",
             "Protocol": "ALL"
           },
           {
             "Port": 443,
             "Hostname": "www.google.com",
             "Protocol": "ALL"
           }
         ]
      }
   }
</code></pre>

<b>Latency</b>
<pre><code>
 "NetworkEmulation": {
      "Duration": 15,
      "EmulationType": "Latency",
      "LatencyDelay" : 1000,
      "RunOrder": 0,
      "TargetEndpoints": {
         "Endpoints": [
           {
             "Port": 443,
             "Hostname": "www.bing.com",
             "Protocol": "ALL"
           },
           {
             "Port": 80,
             "Hostname": "www.msn.com",
             "Protocol": "ALL"
           },
           {
             "Port": 443,
             "Hostname": "www.google.com",
             "Protocol": "ALL"
           }
         ]
      }
   }
</code></pre>

<b>Loss - Random</b>
<pre><code>
 "NetworkEmulation": {
      "Duration": 15,
      "EmulationType": "Loss",
      "LossType": "Random",
      "LossRate" : 0.10,
      "RunOrder": 0,
      "TargetEndpoints": {
         "Endpoints": [
           {
             "Port": 443,
             "Hostname": "www.bing.com",
             "Protocol": "ALL"
           },
           {
             "Port": 80,
             "Hostname": "www.msn.com",
             "Protocol": "ALL"
           },
           {
             "Port": 443,
             "Hostname": "www.google.com",
             "Protocol": "ALL"
           }
         ]
      }
   }
</code></pre>

<b>Loss - Burst</b>
<pre><code>
 "NetworkEmulation": {
      "Duration": 15,
      "EmulationType": "Loss",
      "LossType": "Burst",
      "BurstRate": 0.25,
      "LossRate" : 0.10,
      "RunOrder": 0,
      "TargetEndpoints": {
         "Endpoints": [
           {
             "Port": 443,
             "Hostname": "www.bing.com",
             "Protocol": "ALL"
           },
           {
             "Port": 80,
             "Hostname": "www.msn.com",
             "Protocol": "ALL"
           },
           {
             "Port": 443,
             "Hostname": "www.google.com",
             "Protocol": "ALL"
           }
         ]
      }
   }
</code></pre>

<b>Reorder</b>
<pre><code>
 "NetworkEmulation": {
      "Duration": 15,
      "EmulationType": "Reorder",
      "CorrelationPercentage" : 0.20,
      "PacketPercentage" : 0.10,
      "RunOrder": 0,
      "TargetEndpoints": {
         "Endpoints": [
           {
             "Port": 443,
             "Hostname": "www.bing.com",
             "Protocol": "ALL"
           },
           {
             "Port": 80,
             "Hostname": "www.msn.com",
             "Protocol": "ALL"
           },
           {
             "Port": 443,
             "Hostname": "www.google.com",
             "Protocol": "ALL"
           }
         ]
      }
   }
</code></pre>
