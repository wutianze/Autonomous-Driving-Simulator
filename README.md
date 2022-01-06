<!--
 * @Author: your name
 * @Date: 2020-02-17 20:05:54
 * @LastEditTime: 2020-02-17 20:08:12
 * @LastEditors: Please set LastEditors
 * @Description: In User Settings Edit
 * @FilePath: /Autonomous-Driving-Simulator/README.md
 -->
# This is a light simulator for autonomous driving researchers.

## Download simulator for windows in [baidu cloud](https://pan.baidu.com/s/1xAJ9L7qZGok_46j1IEESgQ), the extract code is `mjtr`. The linux version will be released later.
## The inference server can be run within a docker container: 
1. first docker pull floydhub/tensorflow:1.12-py3_aws.40, then start it
2. docker cp /src into the container
3. docker cp model.h5(you can find a pre-trained one in [HydraMini](https://github.com/wutianze/HydraMini/tree/master/Host-Part/model)) in the 
4. `python3 predict_server.py` to start the service

### Please refer to [HydraMini](github.com/wutianze/HydraMini) for more Information.
