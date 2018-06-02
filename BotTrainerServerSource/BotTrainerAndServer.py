#!/usr/bin/env python

import os
import numpy as np
import pandas as pd
from scipy.misc import imread
from sklearn.metrics import accuracy_score
import tensorflow as tf
import random

import socket 

# To stop potential randomness
seed = 128
rng = np.random.RandomState(seed)



train = []
train_x = []
train_y = []
val_x = []
val_y = []

f = open("DatosPosicion.txt","r", encoding="utf8")
content = f.readlines()
content = [x.strip() for x in content]

i = 0

for line in content:
    line = line.split()
    if(len(line) == 10):
        for j in range(10):
            
            line[j] = float(line[j])
            if(line[j] > 100):
                line[j] = 0.0
            
        train.append((line,0))
    elif(len(line) == 3):
        for j in range(2):
            line[j] = float(line[j])
        if(line[2] == 'False'):
            line[2] = 0.0
        else:
            line[2] = 1.0
        #print(train[i])
        train[i] = (train[i][0],line)
        i+=1             

for x in train:
    train_x.append(x[0])
    train_y.append(x[1])

split_size = int(len(train_x)*0.7)

train_x, val_x = train_x[:split_size], train_x[split_size:]
train_y, val_y = train_y[:split_size], train_y[split_size:]




### set all variables

# number of neurons in each layer
input_num_units = 10
hidden_num_units = 5
output_num_units = 3

# define placeholders
x = tf.placeholder(tf.float32, [None, input_num_units])
y = tf.placeholder(tf.float32, [None, output_num_units])

# set remaining variables
epochs = 4
learning_rate = 0.01

### define weights and biases of the neural network (refer this article if you don't understand the terminologies)

weights = {
    'hidden': tf.Variable(tf.random_normal([input_num_units, hidden_num_units], seed=seed)),
    'output': tf.Variable(tf.random_normal([hidden_num_units, output_num_units], seed=seed))
}

biases = {
    'hidden': tf.Variable(tf.random_normal([hidden_num_units], seed=seed)),
    'output': tf.Variable(tf.random_normal([output_num_units], seed=seed))
}

hidden_layer = tf.add(tf.matmul(x, weights['hidden']), biases['hidden'])
hidden_layer = tf.nn.relu(hidden_layer)

output_layer = tf.matmul(hidden_layer, weights['output']) + biases['output']

cost = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(labels = y, logits = output_layer))

optimizer = tf.train.AdamOptimizer(learning_rate=learning_rate).minimize(cost)

init = tf.global_variables_initializer()

sess = tf.Session()
# create initialized variables
sess.run(init)
    
    ### for each epoch, do:
    ###     create pre-processed batch
    ###     run optimizer by feeding batch
    ###     find cost and reiterate to minimize
    
for epoch in range(epochs):
    avg_cost = 0
    _,c = sess.run([optimizer, cost], feed_dict = {x: train_x, y: train_y})
            
    avg_cost = c
            
    print ("Epoch:", (epoch+1), "cost =", "{:.5f}".format(avg_cost))
    
print ("\nTraining complete!")
    
  
# find predictions on val set
pred_temp = tf.equal(tf.argmax(output_layer, 1), tf.argmax(y, 1))
accuracy = tf.reduce_mean(tf.cast(pred_temp, "float"))
print ("Validation Accuracy:", accuracy.eval(session=sess, feed_dict = {x: val_x, y: val_y}))

host = '' 
port = 50000 
backlog = 5 
size = 1024 
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.bind((host,port)) 
s.listen(backlog) 

while 1:
    client, address = s.accept() 
    print ("Client connected.")
    #client.send(("Hello!\n").encode('utf-8')) 

    while 1: 
        data = client.recv(size)
        
        if data:
            data = data.decode('utf-8')
            data = data.split()
            print("datos recibidos")
            print(data)
            newData = []
            if (len(data) == 10):
                for i in data:
                    
                    if i[0] == '-':
                        newData.append(-float(i[1:]))
                    elif len(i) > 1:
                        if i[0] == '0' and i[1] == '-':
                            newData.append(-float(i[2:]))
                        else:
                            newData.append(float(i))
                    else:
                        newData.append(float(i))
                print("datos limpiados")
                print([newData])
            
                data = sess.run(output_layer, {x:[newData]})
                data = data[0]/10
                print("datos enviados")
                print(data)
                client.send((str(data[0]) + " " + str(data[1])+ " " + str(data[2])).encode('utf-8')) 

sess.close()  

