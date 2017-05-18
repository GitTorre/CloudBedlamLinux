# CloudBedlam Linux (Mono)

#### Simple, configurable, local (VM internal) chaotic operation orchestrator for testing resiliency of Windows-powered cloud services.


### Easy to use 

Step 0.

Just change XML settings to meet your specific chaotic needs. The default config will run CPU, Memory and Networking chaos. You can remove the CPU and Memory nodes and just do Network emulation. For example, the below configuration XML runs Network emulation (Disconnect) for 60 seconds, 2 times successively (Repeat=”1”).
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

Step 1.

Launch CloudBedlam running as sudo:

      sudo CloudBedlam


Have fun and hopefully this proves useful to you in your service resiliency testing. It should be clear that this is a development tool at this stage and not a DevOps workflow orchestrator. You should run this in individual VMs to vet the quality of your code in terms of resiliency and fault tolerance. 

Feel free to supply feedback directly to ctorre@microsoft.com 

## Contributing

Of course, please help make this better 😊 – and add whatever you need around and inside the core chaos engine (which is what this is, really). It may be useful for you to develop a way to deploy configs to local CloudBedlam instances from some remote trusted service (where each side of the secure (443) connection provably trusts the other (shared public key, etc...)). CloudBedlam "agents" would communicate and authenticate with a central Bedlam service which would send down configs that define specific chaos operations for specific sets of VMs in your service(s). Push-based (reactive) interaction with agents would be very cool. 

Obviousy, having a remote-controlled chaotic orchestrator where you can run different tests on different VMs from a central service would be a scalable approach. Like all things in engineering, it comes with a cost, across security and complextity... 

We made this *very simple chaos tool* with a VM-specific approach in mind *specifically to remove the overhead of a distributed secure server-agent model*. The focus for us was on making a *very easy to use, simple to configure, lightweight solution for chaos engineering and experimentation inside virtual machines*.


Make chaos, not war!

…Charles
