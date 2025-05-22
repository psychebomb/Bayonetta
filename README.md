# Let's Dance, Boys!
Adds Bayonetta from the Bayonetta series, a versatile melee-ranged hybrid character able to weave in and out of danger in order to dish out high damage.

Multiplayer Compatible! (from what I've tested)
Feel free to message me (ppsychee) on the RoR2 Modding Discord for any issues that arise, as well as for any general feedback!!

It's highly recommended that you install viliger's <a href="https://thunderstore.io/package/viliger/RevertStunBuff/">RevertStunBuff</a> mod with this mod, as this mod's intended balance relies on the pre-SOTS stun effect. 

## Video

A Special thanks goes to <a href="https://thunderstore.io/package/LONK/">LONK</a> for creating the trailer video for this mod!!

[![BAYONETTA ROR2 MOD TRAILER](https://img.youtube.com/vi/2o8AFYMIa6M/0.jpg)](https://www.youtube.com/watch?v=2o8AFYMIa6M "BAYONETTA ROR2 MOD TRAILER")

## Screenshots
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/select.jpg?raw=true"/>
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/boooom.jpg?raw=true"/>
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/djump.jpg?raw=true"/>
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/flurry.jpg?raw=true"/>
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/bayogif.gif?raw=true"/>

## Skills
### Primary: Bullet Arts
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/texM1.png?raw=true"/>

Perform a 5 hit punch sequence, dealing 200% damage for the first four hits. The last hit is a flurry attack that deals 165% damage each hit and summons a wicked weave when released, dealing 1400% damage. Bayonetta will also continuously fire her guns during the sequence, each shot dealing 50% damage.
If dodge is used during this sequence, Bayonetta will pick up the sequence where she left off as long as M1 is still held. Additionally, using this move against airborne enemies will juggle them, stunning them and knocking them upwards.

#### Secondary: Umbran Techniques
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/texM2.png?raw=true"/>

Perform a variety of different moves depending on movement inputs and whether or not Bayonetta is grounded.
<table>
<thead>
  <tr>
    <th>Input</th>
    <th>Name</th>
    <th>Description</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td>Aerial + Forward</td>
    <td>After Burner Kick</td>
    <td>Launch into the air with a blazing kick, knocking back lighter enemies and dealing 375% damage. Aiming this move downwards will cause it not to launch enemies, but Bayonetta will keep her momentum</td>
  </tr>
  <tr>
    <td>Aerial + Neutral</td>
    <td>Heel Tornado</td>
    <td>Perform a spinning kick, continuously dealing 150% damage. Knocks away lighter enemies. Jump cancellable.</td>
  </tr>
  <tr>
    <td>Aerial + Backward</td>
    <td>Death Drop</td>
    <td>Quickly descend with a downwards kick, continuously dealing 250% damage and spiking enemies.</td>
  </tr>
  <tr>
    <td>Grounded + Forward</td>
    <td>Heel Slide</td>
    <td>Slide forwards at high speed, dealing 100% damage. Continue holding the move down after stopping to perform a rising kick, launching lighter enemies and dealing 300% damage.</td>
  </tr>
  <tr>
    <td>Grounded + Neutral</td>
    <td>Breakdance</td>
    <td>Perform a breakdance, continuously dealing 120% damage per hit to all enemies around you. Juggles airborne opponents similar to M1. Jump cancellable.</td>
  </tr>
  <tr>
    <td>Grounded + Backward</td>
    <td>Full Moon Shoot</td>
    <td>Perform a backflip that launches lighter enemies, dealing 395% damage. Continue holding the move down to keep spinning, continuously dealing 100% damage.</td>
  </tr>
</tbody>
</table>


#### Utility: Dodge
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/texUtility.png?raw=true"/>

Dodge a short distance, gaining brief invincibility and 100 armor. If you are hit during the invincibility, activate **Witch Time.**

While in Witch Time, Gain 350 armor. Enemies and projectiles near Bayonetta are greatly slowed, and moves that normally only launch lighter enemies will now launch all non-boss enemies. Enemies will also have their gravity altered, allowing for new followups.

### Special: Tetsuzanko/Heel Stomp
<img src="https://github.com/psychebomb/Bayonetta/blob/master/uploadthings/texSpecial.png?raw=true"/>

Lock onto enemies and use primary or secondary buttons to summon a wicked weave at their location, dealing 1200% damage.

## Other Interactions
Bayonetta now has new punish attacks!!!!!
- Hitting an enemy with a move that sends downwards (death drop or  heel stomp) will now put enemies in a special stun state.
- If bayonetta is grounded and near an enemy in this state, she will be prompted with the interact button to punish the enemy.
- Mashing the primary skill button (M1) during a punish attack will speed up the attack.
- Two different punish attacks: one for grounded enemies, one for airborne/flying enemies.

Bayonetta has special interactions with items that reduce cooldowns:
- Bandolier: If Witch Time is active, gain an additional 4 seconds when picking up an ammo pack. If Bayonetta has the Witch Time cooldown debuff, remove the debuff upon picking up an ammo pack.
- Brainstalks: Removes all isntances of the Witch Time debuff whenever the no cooldowns buff is active.
- Hardlight Afterburner: Adds 3 seconds to Witch Time's duration.
- Alien Head: Reduces Witch Time's cooldown by 25%
- Light Flux Pauldron: Reduces Witch Time's cooldown by 50%
- Purity: Removes 2 seconds of Witch Time's cooldown.

## Known Issues
- Some animations/sounds don't play properly in multiplayer.
- Enemies killed in witch time will sometimes leave floating corpses
- Buffs are handled a little akwardly for non-host multiplayer users, causing some issues with punish attack invul and some witch time effects

## Todo
- Extra skins
- Extra skills
- Polish VFX
- Other secret stuff

## Credits
- rob + TimeSweeper: For the character template
- PlatinumGames: For making Bayonetta and all the assets in this mod
- DMC and Bayonetta Modding Discord: Extraction tools and tutorials for using Bayonetta 1's assets
- RoR2 Modding Discord: For all the helpful resources :)
- Special thanks goes to my friend Eli for helping me test this mod
