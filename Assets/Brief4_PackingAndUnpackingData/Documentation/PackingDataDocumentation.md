# **packing and Unpacking Data: Specialism Brief 4**

## *Introduction*

In this brief, we consider an old school 3D shooting game. The behaviour to achieve is transfering data packets from a host to a client, and make these packets as small as possible. The host holds all the data at every frame. Players, Pickups and Missile. Each have different sets of information to transfer.

The set fixed limit in packet size is 4 kilobytes per seconds.  This being really small, we have to make sure we only transfer the minimal viable data bits.

Once the data is calculated by the host, a series of conversion to `Bit Array` of every data type is made. Then, we send those arrays to a test client. In turn, the client unpack the Bit Arrays following the same order they have been packed in.

## *Data and Bit Arrays*

We are sending up to four informations per objects:

* **Identification**: A unique number to identify the object.
* **State**: A boolean, true or false, with different meanings depending of the object.
* **Position**: A Vector2 of the position of object. We consider the game to be a shooter with a gameplay on two axis, ignoring the vertical axis.
* **Velocity**: Another Vector2 for velocity.

Each of the objects (Players, Pick-Ups, Missiles) informations will pack it's own chain of *bits*, representing the informations needed, called `Bit Array`. This allows us to control and use a minimum amount of bits per packet.

`Bit Arrays` are actually a series of booleans, packed as bits. False is a value of 0, True is a value of 1. For ease of readings, we consider entries by their bit value rather than their booleans.

## *Data size*

To reduce the number of bits sent, we need to know the size of each types:

* `Bit`: The smallest unit. Value is 0 or 1 only. Obviously using one bit of data.
* `Byte`: One byte is 8 bits, and can represent values from 0 to 255.
* `Short`: Size of 16 bits. Range from -32,768 to 32,767.
* `int`: Size of 32 bits. Range of -2,147,483,648 to 2,147,483,647.

For the **Identification**, we use a `byte`. 256 possible values are way more than enough for the needs. Packed in 8 bits.

For the **State**, we simply use a `boolean` (true or false), which takes up only one bit in a `Bit Array`. Packed in 1 bit.

By default, Vector2 uses two `Integer`, so 64 bits. We will need to transform those into other smaller types as we do not need such a big range.

For the **Position**, we can get away with using two `short`. To get more precision, we can consider dividing the value by 100 once it has been unpacked. This would make a range for each short (Transformed into floats) of -327.68 to 327.67. Packed in 32 bits.

Similarly, for the **Velocity**, we can simply use two `byte`, which when divided by 10, will give us a float with enough range for velocity. Packed in 16 bits.

Once all the *informations* needed have been converted to a `BitArray`, we add them next to each other, making sure we keep the same order. First 8 bits for **Identification**, 9th bit for **state**, next 32 bits fos **Position** if needed (bit 10 to 41), next 16 bits for **Velocity** if needed (bit 42 to 57).

## *Data limit calculation*

We consider a bandwidth limit of **4 kylobytes per seconds**. We also need consider the number of `Tick` by the host, ie: The number of update per seconds that the host send new information to the client. We settle on **30 tick / seconds** for good fluidity.

For the **Players**, we will have two possibilities: If the player is alive (it's state is set to false), the `Bit Array` representing it will have a size of **57 bits**: 8 bits for ID, 1 bit for state, 32 bits for position and 16 bits for velocity. If the Player is dead (it's state is set to true), we skip position and velocity, which result in only 9 bits sent.

For the **Pick-Ups**, We only send ID and updated state every tick. We send the position only once on game startup, and consider every client will store the position locally. Each pick-ups will require only 9 bits per tick.

For the Missile, it's slightly different. Every tick, we check for any new missiles shot by players. Each new missiles needs to be transfered to clients. We send only the *position* and *velocity*, packed in **48 bits**. We consider each players to create a local copy of the missile to keep track of it. We therefore do not need Identification and State sent by the host.

With these informations, we can decide on the numbers of Players and Pickups to stay under the bandwidth limit.

To get as close to the limit, we choose **8 players and 25 pickups max** per game.

8 players needs up to 456 bits per tick if they are all alive (8 x 57), and 25 pickups need 225 bits per tick (25 x 9). If all players fire a missile in the same tick, we need an additional 384 bits (8 x 48).

Added together, the result is a maximum of *1065* bits per tick, which gives us *31,950 bits* per seconds max. Converted to kilobytes (We devide by 8000), we reach a theoretical limit of **3.993 kilobyte / sec**.

## *Creating and Updating Bit Arrays*

At the start of the game, we initialise all the `Bit Arrays` for every objects (By creating arrays of BitArray). We also send the Position of every pickups (just once).

Every tick, we update the arrays of `BitArray`. To update, we scroll through the bits we need to access or change in every `BitArray`, convert the new numbers to the needed type (byte, short, Vector2) and change the value inside the `BitArray`. Note that for players, if the state (9th bit) is set to true, the player is consider dead, so to cut on the position and velocity, we simply set the `Length` of the BitArray to 9, which keeps only the *ID* and *State*.

Regarding the missiles, we update a list of BitArray with the current tick missiles. If no missiles have been shot, the list is cleared (empty).

## *Client reading Datas*

From There, we give the packed data (in BitArray form) to the client. The client then read and unpack the data. To Pack and Unpack certain types, I have created some custom Methods to easily convert from BitArray to Byte, Short, Vector2, and the other way around.

Note that each client will update at the end of the host tick, using the event system.

## *Testing with the Sample Scene*

For testing purposes, we randomize the data every tick. Players have random chance to change status, and random position / velocity. The same applies to pickups and missiles.

The data is then read by the host, packed, send to client and unpacked. The UI shows two views, the host data, and the client data. We also calculate the number of bits sent per ticks, and the kilobytes / sec.

The *Randomize* button on the top left shows another set of players and pickups at random.

Note that all the data can also be read from the inspector, by the **Host** GameOBject and the **ExampleClient** GameObject. Put the game in pause to read and match both datas.
