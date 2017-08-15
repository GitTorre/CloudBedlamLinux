# CloudBedlam for Linux -- Managed (.NET, C#) Impl

# What? Why? How?

### "Chaos Engineering is the discipline of experimenting on a distributed system in order to build confidence in the system’s capability to withstand turbulent conditions in production." 

-From Netflix's Principles of Chaos Engineering Manifesto => http://principlesofchaos.org 

CloudBedlam is a simple, configurable, vm-local chaotic operation orchestrator for resiliency experimentation inside cloud services - chaos, or bedlam as we call it, runs inside individual VMs. At it's core, CloudBedlam causes chaotic conditions by injecting "bedlam" faults (today these are machine resource pressure and network emulation (latency, bandwidth, loss, reorder, disconnection...) into underlying virtual machines that power a service or cluster of services... It is useful for exercising your resiliency design and implementation in an effort to find bugs. It’s also useful for, say, testing your alerting system (e.g., CPU and Memory Azure Alerts) to ensure you set them up correctly or didn’t break them with a new deployment. Network emulation inside the VM enables you to verify that your Internet-facing code handles network faults correctly and/or verify that your solutions to latent or disconnected traffic states work correctly (and help you to refine your design or even establish for the first time how you react to and recover from transient networking problems in the cloud…).

Unlike, say, Netflix's ChaosMonkey, shooting down VM instances isn't the interesting chaos we create with CloudBedlam (though it is definitely interesting, incredibly useful chaos! – just different from what CloudBedlam is designed to help you experiment with…). Instead, we want to add conditions to an otherwise happy VM that make it sad and troubled, turbulent and angry - not just killing it. We believe that there is a useful difference between VMs that are running in configurably chaotic states versus VMs that pseudo-randomly suddenly disappear from the map (again, that is excellent chaos for systems composed of many instances, just not what this tool is designed for. Please add Chaos Monkey to your Chaos Engineering toolset. Netflix are the leaders in this domain and most of their tools are open source, even if baked into Spinnaker today (their open source CI/CD pipeline technology)), you can find very nice non-Spinnaker-embedded versions right here on GitHub...).

This is meant to run chaos experiments <i>inside</i> VMs as a way to experiment close to your code and help you identify resiliency bugs in your design and implementation.


### Note: 

this has only been rigorously tested on Ubuntu 16.04 (Xenial Xerus)... There will be differences in some of the scripts you'll need to take into account for other Linux versions, but for the most part, this should work on most mainline distros (that support Mono and/or .NET Core...) with only a few mods... This impl employs tc for low level network emulation, so it must run as a sudo user.


### Easy to Use

Step 0.

Just change JSON settings to meet your specific chaotic needs. The default config will run CPU, Memory and Networking chaos. You can remove the CPU and Memory objects and just do Network emulation or remove Network and just do CPU/Mem. It's configurable, so do what you want! 

#### Operational Orchestration (Orchestration setting):  

#### Concurrent (run all operations at the same time)  
#### Random (run operations sequentially, in random order)  
#### Sequential (run operations sequentially, based on specified run order (RunOrder))  

#### Currently supported machine resource pressure operations:  

#### CPU (all CPUs) - CpuPressure setting (0 - 100)  
#### Memory (non-swap.. TODO) - MemoryPressure setting (0 - 100)  

#### Currently<a href="https://github.com/GitTorre/CloudBedlamLinux/blob/master/CloudBedlam/NetemReadMe.md" target="new"> supported IPv4/IPV6 network emulation operations</a> - NetworkEmulation - EmulationType setting:

#### Packet Corruption  
#### Packet Loss  
#### Packet Reordering  
#### Bandwidth Rate Limiting  
#### Latency 

#### Network emulation is done for specific endpoints (specified as hostnames) only - Enpoints object (array of Endpoint). CloudBedlam figures out Network type (IPv4 or IPv6) and IPs from hostnames. 

#### NOTE: 
Network emulation requires iproute2 tools (tc and ip, particularly, in CB's case...). This should be present on most mainline distros already, but make sure...

Description: networking and traffic control tools
 The iproute2 suite is a collection of utilities for networking and
 traffic control.

 These tools communicate with the Linux kernel via the (rt)netlink
 interface, providing advanced features not available through the
 legacy net-tools commands 'ifconfig' and 'route'.
Homepage: http://www.linux-foundation.org/en/Net:Iproute2

### Example configuration:

The JSON below instructs CloudBedlam to sequentially run (according to specified run order) a CPU pressure fault of 90% CPU utilization across all CPUs for 15 seconds, Memory pressure fault eating 90% of available memory for 15 seconds, and Network Latency emulation of 1000ms delay for 30 seconds for specified target endpoints (by default, emulation affects both up/downstream connections, so total latency will be 2000ms...). The experiment runs 2 times successively (Repeat=”1”). See [TODO] for more info on available configuration settings, including samples. CloudBedlam will execute (and log) the orchestration of these bedlam operations. You just need to modify some JSON and then experiment away. Enjoy!
<pre><code>
{
  "Orchestration": "Sequential",
  "Duration": "60",
  "RunDelay": "0",
  "Repeat": "1",
  "CpuPressure": {
    "RunOrder": "0",
    "PressureLevel": "90",
    "Duration": "15"
  },
  "MemoryPressure": {
    "RunOrder": "1",
    "PressureLevel": "90",
    "Duration": "15"
  },
  "NetworkEmulation": {
    "RunOrder": "2",
    "EmulationType": "Latency",
    "LatencyDelay" : "1000",
    "TargetEndpoints": {
      "Endpoints": [
        { "Port": "443", "Uri": "https://www.bing.com" },
        { "Port": "80", "Uri": "http://www.msn.com" },
        { "Port": "443", "Uri": "https://www.google.com" }
      ]
    },
    "Duration": "30"
  }
}
</code></pre>


# C++ Version
### If you don't want to program in C# and use the Mono runtime and libraries or .NET Core, you don't have to! C++ developers, please use <a href="https://github.com/GitTorre/CBLinuxN"><b>this version</b></a> and help improve/extend it. 

# .NET Version (this one...)

## Building 

Step 1. (Mono)  

Install MonoDevelop: http://www.monodevelop.com/download/linux/

Step 2:  

Clone project: 

<pre><code>git clone https://github.com/GitTorre/CloudBedlamLinux.git</code></pre>

Open sln in MonoDevelop, build.

## Installing binaries 

If you don't want to install the dependencies and build CloudBedlam, then just grab the latest release and install CloudBedlam and dependencies from a package (file name will be CloudBedlam-linux-amd64):

https://github.com/GitTorre/CloudBedlamLinux/releases [XML config currently]

## Running

CloudBedlam must run as sudo:

      sudo mono CloudBedlam.exe

When running CloudBedlam, a bedlamlogs folder will be created in the folder where the CloudBedlam binary is running. Output file will contain INFO and ERROR data (ERROR info will include error messages and stack traces).

## Contributing

Of course, please help make this better 😊 – and add whatever you need around and inside the core bedlam engine (which is what this is, really). The focus for us is on making a *very easy to use, simple to configure, lightweight solution for chaos engineering and experimentation inside virtual machines*.


Have fun and hopefully this proves useful to you in your service resiliency experimentation. It should be clear that this is a development tool at this stage and not a DevOps workflow orchestrator. You should run this in individual VMs to vet the quality of your code in terms of resiliency and fault tolerance. 


## Feedback

Any and all feedback very welcome. Let us know if you use this and if it helps uncover resiliency/fault tolerance issues in your service implementation. Please <a href="https://github.com/GitTorre/CloudBedlamLinux/issues">create Issues/provide feedback</a>. Thank you! This will continue to evolve and your contributions, in whatever form (words or code), will be greatly appreciated!

--CloudBedlam Team
