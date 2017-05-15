# CloudBedlam Linux (Mono)

#### Simple, configurable, local (VM internal) chaotic operation orchestrator for testing resiliency of Windows-powered cloud services (Linux coming soon). Works on all supported versions of NT-based Windows. Requires .NET 4.5.2. Must be run in Administrator privilege context. Target x64 when you build.

## Using

Step 0.

Clone, build...

Step 1.

Open a Command prompt as Administrator.

NOTE: You must install the network driver first. This guarantees no interuption of networking service when you begin your chaos tests (the driver is a kernel mode filter driver and installing it resets the Windows networking stack…)

Run this command:

      [CloudBedlamBuildPath]\ChaosBinaries\NetEm>NetworkEmulator.exe -install
  
Step 2.

Now, go to the CloudBedlam root folder ([CloudBedlamBuildPath]) and open the Chaos.config file.
Note the comments in the file. Read them 😊

Change the file settings to meet your needs, based on the examples in the XML comment section. The default config will run CPU, Memory and Networking chaos. You can remove the CPU and Memory nodes and just do Network emulation. For example, the below configuration XML runs Network emulation (Disconnect) for 60 seconds, 2 times successively (Repeat=”1”).
<pre><code>
&lt;ChaosConfiguration Orchestration="Sequential" Duration="60" RunDelay="0" Repeat="1"&gt;
	&lt;NetworkEmulation RunOrder="0"&gt;
		&lt;EmulationType&gt;Disconnect&lt;/EmulationType&gt;
		&lt;PeriodicDisconnectionRate&gt;0.7&lt;/PeriodicDisconnectionRate&gt;
		&lt;ConnectionTime&gt;5&lt;/ConnectionTime&gt;
		&lt;DisconnectionTime&gt;15&lt;/DisconnectionTime&gt;
		&lt;TargetEndpoints&gt;
			&lt;Endpoint Port="443" Uri="https://www.bing.com" /&gt;
			&lt;Endpoint Port="80" Uri="http://www.msn.com" /&gt;
			&lt;Endpoint Port="443" Uri="https://www.google.com" /&gt;
		&lt;/TargetEndpoints&gt;
		&lt;Duration&gt;60&lt;/Duration&gt;
	&lt;/NetworkEmulation&gt;
&lt;/ChaosConfiguration&gt;
</code></pre>

Step 3.

Launch an Admin cmd console, run CloudBedlam:

      [CloudBedlamBuildPath]>CloudBedlam 


Have fun and hopefully this proves useful to you in your service resiliency testing. It should be clear that this is a development tool at this stage and not a DevOps workflow orchestrator. You should run this in individual VMs to vet the quality of your code in terms of resiliency and fault tolerance. 

Feel free to supply feedback directly to ctorre@microsoft.com 

## Contributing

Of course, please help make this better 😊 – and add whatever you need around and inside the core chaos engine (which is what this is, really). It may be useful for you to develop a way to deploy configs to local CloudBedlam instances from some remote trusted service (where each side of the secure (443) connection provably trusts the other (shared public key, etc...)). CloudBedlam "agents" would communicate and authenticate with a central Bedlam service which would send down configs that define specific chaos operations for specific sets of VMs in your service(s). Push-based (reactive) interaction with agents would be very cool. 

Obviousy, having a remote-controlled chaotic orchestrator where you can run different tests on different VMs from a central service would be a scalable approach. Like all things in engineering, it comes with a cost, across security and complextity... 

We made this *very simple chaos tool* with a VM-specific approach in mind *specifically to remove the overhead of a distributed secure server-agent model*. The focus for us was on making a *very easy to use, simple to configure, lightweight solution for chaos engineering and experimentation inside virtual machines*.


Make chaos, not war!

…Charles
