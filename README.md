# CloudBedlam for Linux (Mono or .NET Core)

#### Simple, configurable, local (VM) chaotic operation orchestrator for measuring the resiliency of cloud services by injecting bedlam (machine resource and networking faults) into underlying virtual machines. This version is meant to run inside Linux VMs (and containers).


### Easy to use 

Step 0.

Just change XML settings to meet your specific chaotic needs. The default config will run CPU, Memory and Networking chaos. You can remove the CPU and Memory nodes and just do Network emulation or remove Network and just do CPU/Mem. It's configurable, so do what you want! 

For example, the below configuration XML sequentially runs (according to specified run order) a CPU pressure fault of 90% CPU utilization across all CPUs for 15 seconds, Memory pressure fault eating 90% of available memory for 15 seconds, and Network Latency emulation fault of 3000ms delay for 30 seconds for specified target endpoints. The experiment runs 2 times successively (Repeat=”1”). See Chaos.config for more info on available configuration settings. CloudBedlam will execute (and log) the orchestration of these chaos operations. You just need to modify some XML and then test away. Enjoy!
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

Step 1. (Mono)

Launch CloudBedlam running as sudo:

      sudo mono CloudBedlam.exe

A bedlamlogs folder will be created in the folder where the CloudBedlam binary is running. Output file will contain INFO and ERROR lines (ERROR lines will include error messages and stack traces).

===

Have fun and hopefully this proves useful to you in your service resiliency testing. It should be clear that this is a development tool at this stage and not a DevOps workflow orchestrator. You should run this in individual VMs to vet the quality of your code in terms of resiliency and fault tolerance.

## Contributing

Of course, please help make this better 😊 – and add whatever you need around and inside the core bedlam engine (which is what this is, really). The focus for us is on making a *very easy to use, simple to configure, lightweight solution for chaos engineering and experimentation inside virtual machines*.


Make bedlam, not war!

