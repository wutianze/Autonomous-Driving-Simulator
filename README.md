# This is a light simulator for autonomous driving researchers.

## Download simulator for windows in [baidu cloud](https://pan.baidu.com/s/1xAJ9L7qZGok_46j1IEESgQ), the extract code is `mjtr`. The linux version will be released later.
## The inference server can be run within a docker container: 
1. first docker pull floydhub/tensorflow:1.12-py3_aws.40, then start it
2. docker cp /src into the container
3. docker cp model.h5(you can find a pre-trained one in [HydraMini](https://github.com/wutianze/HydraMini/tree/master/Host-Part/model)) too.
4. `python3 predict_server.py` to start the service
## [Doc](https://app.gitbook.com/s/-Ls1LnOsBMf7xZoFS0Ml/virtual-part/virtual-guide)

### Please refer to [HydraMini](github.com/wutianze/HydraMini) for more Information.
