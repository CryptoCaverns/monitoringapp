for i in {228..254}; do 
	ssh -t ethos@192.175.10.$i "sudo allow && r"
done