 # CloudBedlam for Linux (Mono or .NET Core)

#### Simple, configurable, local (VM) chaotic operation orchestrator for measuring the resiliency of cloud services by injecting bedlam (machine resource and networking faults) into underlying virtual machines. This version is meant to run inside Linux VMs (and containers).


### Easy to use 

Step 0.

Just change XML settings to meet your specific chaotic needs. The default config will run CPU, Memory and Networking chaos. You can remove the CPU and Memory nodes and just do Network emulation or remove Network and just do CPU/Mem. It's configurable, so do what you want! 

Currently supported network emulation operations:

Packet Corruption  
Packet Loss  
Packet Reordering  
Bandwidth Rate Limiting  
Latency  

For example, the below configuration XML sequentially runs (according to specified run order) a CPU pressure fault of 90% CPU utilization across all CPUs for 15 seconds, Memory pressure fault eating 90% of available memory for 15 seconds, and Network Latency emulation fault of 3000ms delay for 30 seconds for specified target endpoints. The experiment runs 2 times successively (Repeat=”1”). See Chaos.config for more info on available configuration settings, including samples. CloudBedlam will execute (and log) the orchestration of these bedlam operations. You just need to modify some XML and then experiment away. Enjoy!
<pre><code>
&lt;ChaosConfiguration Orchestration="Sequential" Duration="60" RunDelay="0" Repeat="1"&gt;
	&lt;CpuPressure RunOrder="0"&gt;
        	&lt;PressureLevel&gt;90&lt;/PressureLevel&gt;
        	&lt;Duration&gt;15&lt;/Duration&gt;
	&lt;/CpuPressure&gt;
	&lt;MemoryPressure RunOrder="1"&gt;
        	&lt;PressureLevel&gt;90&lt;/PressureLevel&gt;
        	&lt;Duration&gt;15&lt;/Duration&gt;
	&lt;/MemoryPressure&gt;
	&lt;NetworkEmulation RunOrder="2"&gt;
		&lt;EmulationType&gt;Latency&lt;/EmulationType&gt;
		&lt;LatencyDelay&gt;3000&lt;/LatencyDelay&gt;
		&lt;TargetEndpoints&gt;
			&lt;Endpoint Port="443" Uri="https://www.bing.com" /&gt;
			&lt;Endpoint Port="80" Uri="http://www.msn.com" /&gt;
			&lt;Endpoint Port="443" Uri="https://www.google.com" /&gt;
		&lt;/TargetEndpoints&gt;
		&lt;Duration&gt;30&lt;/Duration&gt;
	&lt;/NetworkEmulation&gt;
&lt;/ChaosConfiguration&gt;
</code></pre>

## Building 

Step 1. (Mono)  

Install MonoDevelop: http://www.monodevelop.com/download/linux/

Step 2:  

Clone project: 

<pre><code>git clone https://github.com/GitTorre/CloudBedlamLinux.git</code></pre>

Open sln in MonoDevelop, build.

## Installing binaries 

If you don't want to install the dependencies and build CloudBedlam, then just grab the latest release and install CloudBedlam and dependencies from a package (file name will be CloudBedlam-linux-amd64):

https://github.com/GitTorre/CloudBedlamLinux/releases

## Running

CloudBedlam must run as sudo:

      sudo mono CloudBedlam.exe

When running CloudBedlam, a bedlamlogs folder will be created in the folder where the CloudBedlam binary is running. Output file will contain INFO and ERROR lines (ERROR lines will include error messages and stack traces).

## Contributing

Of course, please help make this better 😊 – and add whatever you need around and inside the core bedlam engine (which is what this is, really). The focus for us is on making a *very easy to use, simple to configure, lightweight solution for chaos engineering and experimentation inside virtual machines*.


Have fun and hopefully this proves useful to you in your service resiliency experimentation. It should be clear that this is a development tool at this stage and not a DevOps workflow orchestrator. You should run this in individual VMs to vet the quality of your code in terms of resiliency and fault tolerance. 


## Feedback

Any and all feedback very welcome. Let us know if you use this and if it helps uncover resiliency/fault tolerance issues in your service implementation. You can send mail to ctorre@microsoft.com and/or <a href="https://github.com/GitTorre/CloudBedlamLinux/issues">create Issues here</a>. Thank you! This will continue to evolve and your contributions, in whatever form (words or code), will be greatly appreciated!



Make bedlam, not war!

--CloudBedlam Team
