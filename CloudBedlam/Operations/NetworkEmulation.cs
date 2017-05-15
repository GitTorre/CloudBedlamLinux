using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using CloudBedlam.Config;
using CloudBedlam.Extensions;

/*
NOTE: Linux netem sample for delay:
sudo tc qdisc del dev wlp2s0 root netem rate 5kbit 20 100 5

Start...
charles@charles-HP-ZBook-Studio-G3:~$ sudo tc qdisc add dev wlp2s0 root netem delay 1000ms
Stop
charles@charles-HP-ZBook-Studio-G3:~$ sudo tc qdisc del dev wlp2s0 root


wlp2s0 is the id of the wireless radio in my dev machine... So, we need to first query for 
network devices (and I'd imagine there is a way to just say "all"?...) 

useful commmands:

ifconfig

links to learn more...

http://www.tecmint.com/linux-network-configuration-and-troubleshooting-commands/
https://www.excentis.com/blog/use-linux-traffic-control-impairment-node-test-environment-part-1
http://manpages.ubuntu.com/manpages/wily/man8/tc-netem.8.html#contenttoc5


EXAMPLES:

Latency and jitter

Using the netem qdisc we can emulate network latency and jitter on all outgoing packets (man page, read more). Some examples:

$ tc qdisc add dev eth0 root netem delay 100ms
<delay packets for 100ms>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms
<delay packets with value from uniform [90ms-110ms] distribution>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms 25%
<delay packets with value from uniform [90ms-110ms] distribution and 25% \
    correlated with value of previous packet>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms distribution normal
<delay packets with value from normal distribution (mean 100ms, jitter 10ms)>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms 25% distribution normal
<delay packets with value from normal distribution (mean 100ms, jitter 10ms) \
    and 25% correlated with value of previous packet>


Packet loss

Using the netem qdisc packet loss can be emulated as well (man page, read more). Some simple examples:

$ tc qdisc add dev eth0 root netem loss 0.1%
<drop packets randomly with probability of 0.1%>

$ tc qdisc add dev eth0 root netem loss 0.3% 25%
<drop packets randomly with probability of 0.3% and 25% correlated with drop \
    decision for previous packet>
But netem can even emulate more complex loss mechanism, such as the Gilbert-Elliot scheme. This scheme defines 2 states Good (or drop Gap) and Bad (or drop Burst). The drop chances of both states and the chances of switching between states are all provided. See section 3 of this paper for more info.

$ tc qdisc add dev eth0 root netem loss gemodel 1% 10% 70% 0.1%
<drop packets using Gilbert-Elliot scheme with probabilities \
    move-to-burstmode (p) of 1%, move-to-gapmode (r) of 10%, \
    drop-in-burstmode (1-h) of 70% and drop-in-gapmode (1-k) of 0.1%>


Specific IP filtering...

#!/bin/bash

interface=lo
ip=10.0.0.1
delay=100ms

tc qdisc add dev $interface root handle 1: prio
tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dst $ip flowid 2:1
tc qdisc add dev $interface parent 1:1 handle 2: netem delay $delay

*/


namespace CloudBedlam.Operations
{
    internal class NetworkEmulation : OperationBase
    {
        private readonly Config.NetworkEmulation _config;
        
        public NetworkEmulation(Config.NetworkEmulation config, TimeSpan testDuration) 
            : base(config.IsValidProfile() && config.DurationInSeconds > 0,
                  config.Duration > testDuration ? testDuration : config.Duration,
                  config.RunOrder)
        {
            _config = config;
        }

		/*
			We only really care about emulating outgoing/incoming traffic for specific IPs... -CT
			
			Linux =>
			From https://wiki.linuxfoundation.org/networking/netem
			
			Here is a simple example that only controls traffic to one IP address.

			 # tc qdisc add dev eth0 root handle 1: prio
			 # tc qdisc add dev eth0 parent 1:3 handle 30: \
			tbf rate 20kbit buffer 1600 limit  3000
			 # tc qdisc add dev eth0 parent 30:1 handle 31: \
			netem  delay 200ms 10ms distribution normal
			 # tc filter add dev eth0 protocol ip parent 1:0 prio 3 u32 \
			     match ip dst 65.172.181.4/32 flowid 1:3


			How can I use netem on incoming traffic?

			You need to use the Intermediate Functional Block pseudo-device IFB . This network device allows attaching queuing discplines to incoming packets.

			 # modprobe ifb
			 # ip link set dev ifb0 up
			 # tc qdisc add dev eth0 ingress
			 # tc filter add dev eth0 parent ffff: \ 
			   protocol ip u32 match u32 0 0 flowid 1:1 action mirred egress redirect dev ifb0
			 # tc qdisc add dev ifb0 root netem delay 750ms
		*/

		//TODO: Change this to support tc/netem.... CT
        protected override ProcessParams CreateProcessParams()
        {
            var config = GetEmulationConfiguration(_config);

            if (config == null) return null;

            var args = "";
            //Latency
            var latencyConfig = config as LatencyConfiguration;
            if (latencyConfig != null)
            {
                args = "-config latency -delay " + latencyConfig.FixedLatencyDelayMilliseconds + " -url " + GetEndpointPortString(latencyConfig.TargetEndpoints.Endpoints) + " -duration " +
                       _config.DurationInSeconds;
            }
            //Bandwidth
            var bandwidthConfig = config as BandwidthConfiguration;
            if (bandwidthConfig != null)
            {
                args = "-config bandwidth -dsbandwidth " + bandwidthConfig.DownstreamBandwidth + " -usbandwidth " +
                       bandwidthConfig.UpstreamBandwidth + " -url " + GetEndpointPortString(bandwidthConfig.TargetEndpoints.Endpoints) + " -duration " +
                       _config.DurationInSeconds;
            }
            //Disconnect
            var disconnectConfig = config as DisconnectConfiguration;
            if (disconnectConfig != null)
            {
                 /*
                 -config disconnect -connectiontime 5 -disconnectiontime 15 -disconnectionrate 0.8 -url https://www.bing.com -duration 15
                 */
                args = "-config disconnect -connectiontime " + disconnectConfig.ConnectionTime + " -disconnectiontime " + disconnectConfig.DisconnectionTime + " -disconnectionrate " + disconnectConfig.PeriodicDisconnectionRate + " -url " + GetEndpointPortString(disconnectConfig.TargetEndpoints.Endpoints) + " -duration " + _config.DurationInSeconds;
            }
            //Loss
            var lossConfig = config as LossConfiguration;
            if (lossConfig != null)
            {
                //-config loss -losstype random -lossrate 0.9 -protocol udp -url https://www.bing.com -duration 15
                string loss = "";
                switch (_config.LossType)
                {
                    case LossType.Burst:
                        loss = " -lossrate " + lossConfig.BurstRate;
                        break;
                    case LossType.Random:
                        loss = " -lossrate " + lossConfig.RandomLossRate;
                        break;
                    case LossType.Periodic:
                        loss = " -lossperiod " + lossConfig.PeriodicLossPeriod;
                        break;
                }
                args = "-config loss -losstype " + lossConfig.LossType + loss + " -protocol " + lossConfig.ProtocolLayerType + " -url " + GetEndpointPortString(lossConfig.TargetEndpoints.Endpoints) + " -duration " + _config.DurationInSeconds;
            }

            return new ProcessParams(null, args);
        }

        private static string GetEndpointPortString(IEnumerable<Endpoint> endpoints)
        {
            string value = "";
            foreach (var endpoint in endpoints)
            {
                string port = "";
                if (!string.IsNullOrEmpty(endpoint.Port))
                {
                    port = ":" + endpoint.Port;
                }
                value += endpoint.Uri + port + ",";
            }
            return value.TrimEnd(',');
        }

        private EmulationConfiguration GetEmulationConfiguration(Config.NetworkEmulation config)
        {
            EmulationConfiguration emulationConfiguration = null;

            if (config.EmulationType == NetworkEmProfile.Bandwidth)
            {
                emulationConfiguration = new BandwidthConfiguration
                {
                    DownstreamBandwidth = config.BandwidthDownstreamSpeed,
                    UpstreamBandwidth = config.BandwidthUpstreamSpeed,
                    TargetEndpoints = config.TargetEndpoints
                };
            }
            if (config.EmulationType == NetworkEmProfile.Latency)
            {
                emulationConfiguration = new LatencyConfiguration
                {
                    FixedLatencyDelayMilliseconds = _config.LatencyDelay,
                    TargetEndpoints = config.TargetEndpoints
                };
            }
            if (config.EmulationType == NetworkEmProfile.Disconnect)
            {
                emulationConfiguration = new DisconnectConfiguration
                {
                     ConnectionTime = config.ConnectionTime,
                     DisconnectionTime = config.DisconnectionTime,
                     PeriodicDisconnectionRate = config.PeriodicDisconnectionRate,
                     TargetEndpoints = config.TargetEndpoints
                };
            }
            if (config.EmulationType == NetworkEmProfile.Loss)
            {
                emulationConfiguration = new LossConfiguration
                {
                    BurstRate = config.BurstRate,
                    MaximumBurst = config.MaximumBurst,
                    MinimumBurst = config.MinimumBurst,
                    PeriodicLossPeriod = config.PeriodicLossPeriod,
                    RandomLossRate = config.RandomLossRate,
                    TargetEndpoints = config.TargetEndpoints,
                    ProtocolLayerType = config.ProtocolLayerType,
                    NetworkLayerType = config.NetworkLayerType
                };
            }

            return emulationConfiguration;
        }

		//TODO: Play with sudo...
		// FileName = "/usr/bin/sudo";
		// Arguments = "/usr/bin/tc qdisc add dev wlp2s0 root netem delay 1000ms";
		private Tuple<string, string> RunNetworkEmulation()
		{
			Process process = new Process();
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = "sudo";
			processStartInfo.Arguments = "tc qdisc add dev wlp2s0 root netem delay 1000ms";
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.Verb = "RunAs";
			process.StartInfo = processStartInfo;

			process.Start();
			string error = process?.StandardError.ReadToEnd();
			string output = process?.StandardOutput.ReadToEnd();
			process.WaitForExit(_config.Duration.Milliseconds);

			StopNetworkEmulation();

			var tuple = new Tuple<string, string>(error, output);
			return tuple;
		}
		//TODO: Play with sudo...
		// FileName = "/usr/bin/sudo";
		// Arguments = "/usr/bin/tc qdisc del dev wlp2s0 root";
		private void StopNetworkEmulation()
		{
			Process process = new Process();
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = "sudo";
			processStartInfo.Arguments = "tc qdisc del dev wlp2s0 root";
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.Verb = "RunAs";
			process.StartInfo = processStartInfo;
			process.Start();
		}

		private static IEnumerable<IPAddress> GetIpAddressesForEndpoint(string hostname)
		{
			try
			{
				return Dns.GetHostEntry(hostname).AddressList;
			}
			catch (SocketException)
			{
				return null;
			}
		}

        internal override void Kill()
        {
            if (Process == null || !Process.IsRunning() || Process.HasExited) return;

            System.Threading.Thread.Sleep(5000); //Give the emulator time to stop and uninstall net driver...
            Process?.Kill();
        }
    }
}
