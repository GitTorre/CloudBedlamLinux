# CloudBedlam for Linux -- Managed (.NET, C#) Impl

# What? Why? How?

### "Chaos Engineering is the discipline of experimenting on a distributed system in order to build confidence in the system’s capability to withstand turbulent conditions in production." 

-From Netflix's Principles of Chaos Engineering Manifesto => http://principlesofchaos.org 

CloudBedlam is a simple, configurable, vm-local chaotic operation orchestrator for resiliency experimentation inside virtual machines. Configurable chaotic conditions are induced via fault injections and run inside individual VMs. It's an easy to use Chaos Engineering tool for running chaos experiments <i>close to your service binaries</i> - in the virtual machines where your services are running. This guarantees isolation (faults will only impact your VMs and therefore only your services will be impacted).

CloudBedam is useful for exercising your resiliency design and implementation under realistic failure conditions in an effort to find flaws and to better understand how your cloud services behave in realisitic cloud failure scenarios. It’s also useful for, say, testing your alerting system (e.g., CPU and Memory Alerts) to ensure you set them up correctly or didn’t break them with a new deployment. 

Network emulation inside a VM enables you to verify that your Internet-facing code handles network faults correctly and/or verify that your solutions to latent or disconnected traffic states work correctly. This will help you to refine your design or even establish for the first time how you react to and recover from transient networking problems in the cloud….

Unlike, say, Netflix's ChaosMonkey, shooting down VM instances isn't the interesting chaos you create with CloudBedlam (though it is definitely interesting, incredibly useful chaos! – just different from what CloudBedlam is designed to help you experiment with…). Instead, CloudBedlam adds conditions to an otherwise happy VM that make it sad and troubled, turbulent and angry - not just killing it. 

Hypothesis:  

<i>There is a useful difference between VMs that are running in configurably chaotic states versus VMs that pseudo-randomly disappear from the map.</i>  

Please make sure to add Chaos Monkey to your Chaos Engineering toolset. Netflix are the leaders in the Chaos Engineering domain (they invented the discipline!) and most of their tools are open source, even if baked into Spinnaker today (their open source CI/CD pipeline technology)). You can find very nice non-Spinnaker-embedded versions right here on GitHub, including an <a href="https://github.com/mlafeldt/docker-simianarmy" target="_blank">almost-full Simian Army that has been containerized</a>!

CloudBedlam is meant to run chaos experiments <i>inside</i> VMs as a way to experiment close to your code and help you identify resiliency bugs in your design and implementation.


### Note: 

This has only been rigorously tested on Ubuntu 16.04 (Xenial Xerus)... There will be differences in some of the scripts you'll need to take into account for other Linux versions, but for the most part, this should work on most mainline distros (that support Mono and/or .NET Core...) with only a few mods... This impl employs tc for low level network emulation, so it must run as a sudo user.

Obviously, you need to be <i><b>allowed</b></i> (so, by policy on your team...) to run non-service binaries onboard the VMs that host your cloud services. Further, due to network emulation via tc, which runs code inside the kernel, you have to run CB as a sudo user. 

Currently, there is no big red button (Stop) implemented, but it will be coming as this evolves into a system that can be remote controlled over SSH by a mutually trusted service (where having a big red button really matters given in that case you'd probably have multiple VMs running CB...)... In this impl, you control lifetimes of chaotic operations via configuration settings (duration) for each operation you want to use to induce controlled, directed, deterministic chaos as part of your chaos experimentation.


### Easy to Use

Step 0.

Just change JSON settings to meet your specific chaotic needs. The default config will run CPU, Memory and Networking chaos. You can remove the CPU and Memory objects and just do Network emulation or remove Network and just do CPU/Mem. It's configurable, so do what you want! 

#### ChaosConfiguration properties:

       Delay - Time in seconds to wait until starting chaotic operations  
       Repeat - Number of times to repeat a complete run  
       Orchestration - How to run through the operations    
              --> Concurrent (run all operations at the same time)  
              --> Random (run operations sequentially, in random order)  
              --> Sequential (run operations sequentially, based on specified run orders)  

#### Currently supported machine resource pressure operations:  

       CpuPressure (all CPUs)  
       MemoryPressure (non-swap)  
       
       Cpu/MemoryPressure json object properties:  
              --> PressureLevel - Percentage pressure level (0 - 100)
              --> Duration - Time in seconds to run the operation  
              --> RunOrder - 0 based run order specifier (for Sequential orchestration ordering)  

#### Currently<a href="https://github.com/GitTorre/CloudBedlamLinux/blob/master/CloudBedlam/NetemReadMe.md" target="new"> supported IPv4/IPV6 network emulation operations</a> - (NetworkEmulation EmulationType settings)

       NetworkEmulation json object properties:  
       Duration - Time in seconds to run the operation  
       RunOrder - 0 based run order specifier (if you don't supply this, the system will take care of ordering)  
       EmulationType - The type of emulation to run...  
             --> Bandwidth 
             --> Corruption
             --> Latency 
             --> Loss  
             --> Reorder 
       Endpoints - target endpoints to affect with emulation    
              --> Hostname - host to affect with emulation  
              --> Port - Port to affect with emulation
              --> Protocol (see below) - The network protocol to affect with emulation
       
#### TODO (right now, it's ALL...) Network Protocols:  

       ALL
       ICMP
       TCP
       UDP
       ESP
       AH
       ICMPv6

#### Network emulation is done for specific endpoints (specified as hostnames) only - Endpoints object. CloudBedlam determines currrent Network type (IPv4 or IPv6) and IPs from hostnames. 

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

The JSON below instructs CloudBedlam to sequentially run (according to specified run order) a CPU pressure fault of 90% CPU utilization across all CPUs for 15 seconds, Memory pressure fault eating 90% of available memory for 15 seconds, and Network Latency emulation of 1000ms delay for 30 seconds for specified target endpoints. The experiment runs 2 times successively (Repeat=”1”). CloudBedlam will execute (and log) the orchestration of these bedlam operations. You just need to modify some JSON and then experiment away. Enjoy!
<pre><code>
{
  "Orchestration": "Sequential",
  "Repeat": 1,
  "CpuPressure": {
    "PressureLevel": 90,
    "Duration": 15,
    "RunOrder": 0
  },
  "MemoryPressure": {
    "PressureLevel": 80,
    "Duration": 15,
     "RunOrder": 1
  },
  "NetworkEmulation": {
    "Duration": 30,
    "RunOrder": 2,
    "EmulationType": "Latency",
    "LatencyDelay" : 1000,
    "TargetEndpoints": {
      "Endpoints": [
        {
          "Hostname": "www.bing.com", 
          "Port": 443, 
          "Protocol": "ALL" 
        },
        { 
          "Hostname": "www.msn.com", 
          "Port": 80, 
          "Protocol": "ALL" 
        },
        { 
          "Hostname": "www.google.com", 
          "Port": 443, 
          "Protocol": "ALL" 
        }
      ]
    }
  }
}
</code></pre>


# C++ Version
### If you don't want to program in C# and use the Mono runtime and libraries or .NET Core, you don't have to! C++ developers, please use <a href="https://github.com/GitTorre/CBLinuxN"><b>this version</b></a> and help improve/extend it. 

# .NET Version (this one...)

## Building 

Step 1. (Mono)  

Install MonoDevelop: http://www.monodevelop.com/download/linux/  
OR  
Install Rider: https://www.jetbrains.com/rider/

Step 2:  

Clone project: 

<pre><code>git clone https://github.com/GitTorre/CloudBedlamLinux.git</code></pre>

Open sln in MonoDevelop OR Rider, build.

### NOTE: Releases are not up to date. It's best you follow the directions to build from sources in order to get the latest implementation and then keep up to date with pulls...

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
