# Documentation for Unity Tutorial

## set up Github with Unity
- Create new Unity project
- Github new repository
- Give it a Name and Description
- Local Path inside the Unity Folder (Look for Asset)
- Git ignore --> Unity
- Make sure to have the .gitignore in the Unity Folder (in case it doesn't work like usually)  
- If you know already what you will need, you can already prepare some Folders. (Scripts, Material, Prefabs, Debug)
- new Branches for every major thing (Player, Enemies, Items etc.)

## Moving Player
- Make a new Scene  
- Place Floor and Player  
- Make Material for funny and better sight  
- Make the Player a Prefabs ASAP  
- Before forgetting, If you need Collision -> GIVE RIGIDBODY  
- Check the Constraints in Rigidbody  
- **Nice Time to Commit 👁️👁️**  
- Create Player Input  
- Action Map for Player thing, Actions for the Input thing  
- Depending on what you want, either Vector or just a Button press different things. Just try things out.  
- **Save.**  
- Back again to Player and give Player Input  
- Hey we finally need our Script, so make a Script and call it PlayerMovement or something  
- What was the parameter inside the function again? IDK Google it, look at documentation or ask ChatGPT (worse option). But yeah its
```C#
public void GetPlayerMovement(InputAction.CallbackContext context)
{
    // Do Things
}
```
look at context and see what it offers. Good idea for beginners.  
Read the Value of context and put it inside a Vector3 variable. Like this:
```C#
Vector3 movement = new Vector3()
// ...
public void GetPlayerMovement(InputAction.CallbackContext context)
{
    movement.x = context.ReadValue<Vector2>().x;
    movement.z = context.ReadValue<Vector2>().y;
}
```
In Update move the Player, by coding "transform.position += movement;".  
Maybe put the camera into the Player's Prefab.  
Put the Script in the Parent and connect the Player Input with it.  
**Nice Time to Commit 👁️👁️**  

> Wow the Player is super fast, yeah because we move 1 Unit per Frame. Let's add the Time.deltaTime into it. so:
```C#
private void Update()
{
    transform.position += movement * Time.deltaTime;
}
```
> Holy shit are we moving slowy! Yes because we are moving now 1 Unit per second. Sooo to counter that we make a speed variable like this:
 ```C#
//Either
[SerializeField] float speed;
// or
public float speed;

Vector 3 movement = new Vector3()
// ...
private void Update()
{
    transform.position += movement * Time.deltaTime * speed;
}
```
Now we can change the speed value live in the Unity Editor.

> [!TIP]
> Don't forget that changes aren't being saved in gaming mode and it won't change it on the Prefab if you don't apply it directly into the Prefab or override it.
 
**Nice Time to Commit 👁️👁️**  

## Rotating the Player
We want the Player to look at the direction where the mouse is looking at, first I am going to give him some eyes so that we really can see the rotation.

Now you can take the Camera to the `PlayerMovement` and let it there check. But, its the Player Movement and it can be quite confusing. Since it's a GameJam we won't be very clean with our methods and code, but we can try a bit at least.  

We are going to take the ScreenPoint of the Mouse in the Screen and make it into a Ray. This Ray will then be fired into the world and collide with something in the Space. We then get a `Vector3` which we can than use to make our Player look at that Vector.    

So we make a extra CameraScript and give it to the camera of the Player. The Code inside looks like this:
```C#
private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray,out RaycastHit hitInfo))
        {
            Vector3 lookPoint = hitInfo.point;
            lookPoint.y = 0;
        }
    }
```
Now we would like to give this Information to our `PlayerMovement`, and we could reference it to the GameObject but that looks awful. A much nicer approach is a middleman that stores data for the Player.

So we will be using [**Scriptable Objects**](https://gamedevbeginner.com/scriptable-objects-in-unity/)     
- Right-click into the Project tab and make a Script, but this time not `MonoBehaviour` but `ScriptableObject`.   
- Inside we put variables that we would like to use.   
- Like the Vector3 of the lookpoint, or also our speed if we want to change it via enemies behavior or powerups.    

After adding our lookPoint variable into the ScritableObject script we go back to our CameraScript and add this:
```C#
// Add this
[SerializeField] PlayerData data;

private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.value;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePosition);
        if(Physics.Raycast(ray,out RaycastHit hitInfo))
        {
            Vector3 lookPoint = hitInfo.point;
            lookPoint.y = 0;    
            // And add this
            data.lookPoint = lookPoint;
        }
    }
```
In our Unity Project tab we create our Object of our ScriptableObject and give it to the camera.  
Now if we start the game and look at the PlayerData we should see that the values are changing, and these Values will be then be taken by the Player, but you know what would be a great thing to do right now: 

**Nice Time to Commit 👁️👁️**  

Alright now make a PlayerLook Script and add it into the body NOT into the empty object since it would turn the camera.   
And add this little Code:
```C#
[SerializeField] PlayerData data;

private void Update()
{
    Vector3 position = transform.position;
    transform.foward = data.lookPoint - new Vector3(position.x,0,position.z);
}
```
> And now we look at where the mouse is. Crazy right? You didn't forget to give the PlayerLook Script our Scriptable Object now did you?👁️👁️  
 
**Nice Time to Commit 👁️👁️**  

## Making our first Enemy
Since we are charting new territory we will make a new Branch  

And to make it GameJam-realisitc we are gonna cosplay another programmer who worked at the enemy at the same time our Player programmer and branch from main.  
Same with the Player, make a new Test Scene, make a prefab, give him some funny texture and so on.  

**Nice Time to Commit 👁️👁️**

Next up, how about making the Enemy move Towards a specific position that we could later on replace with the Player position.  
So once again, make a Folder for Scripts -> Enemy and make a new `MonoBehaviour`.  

Alright the Code looks like this:
```C#
    [SerializeField] Vector3 PlayerPosition;
    [SerializeField] float Speed;

    private void Update()
    {
        Vector3 direction = Vector3.Normalize(PlayerPosition - transform.position);
        direction.y = 0;
        transform.forward = direction;
        transform.position += direction * Time.deltaTime * Speed;
        
    }
```
To make it more interesting how about we make the position random every 2 seconds.  
This will be our first time we try to make a cooldown and we have two ways of going about it:
```C#
    //[SerializeField] Vector3 PlayerPosition;
    [SerializeField] float Speed;
    [SerializeField] float minX,maxX,minZ,maxZ;
    [SerializeField] float Cooldown;
    Vector3 PlayerPosition;
    // Cooldown way 1
    float lastTime;
    // Cooldown way 2
    float cooldownTimer;

    private void Start()
    {
        // Cooldown 1
        lastTime = Time.time;
        // Cooldown 2
        cooldownTimer = 0;
    }

    // Cooldown 1
    private void Update()
    {
        if (lastTime + Cooldown < Time.time)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            PlayerPosition = new Vector3(randomX, 0, randomZ);
            Debug.Log(PlayerPosition);
            lastTime = Time.time;
        }

        Vector3 direction = Vector3.Normalize(PlayerPosition - transform.position);
        direction.y = 0;
        //Debug.Log($"Direction: {direction}");
        if(direction.x != 0 && direction.z != 0)
        {
            transform.forward = direction;
            transform.position += direction * Time.deltaTime * Speed;
        }
    }

    // Cooldown 2
    private void Update()
    {
        if (cooldownTimer > Cooldown)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            PlayerPosition = new Vector3(randomX, 0, randomZ);
            Debug.Log(PlayerPosition);
            cooldownTimer = 0;
        }

        Vector3 direction = Vector3.Normalize(PlayerPosition - transform.position);
        direction.y = 0;
        //Debug.Log($"Direction: {direction}");
        if(direction.x != 0 && direction.z != 0)
        {
            transform.forward = direction;
            transform.position += direction * Time.deltaTime * Speed;
        }
        cooldownTimer += Time.deltaTime;
    }
```
Which Cooldown you should use is more of a situation thing (with Time.deltaTime you can show it better since you have a counter that you can display through UI).  
In the end both do the job.

Alright we can move towards a position, now we should also look for collision, but before that:

**Nice Time to Commit 👁️👁️** 


To simulate the Player, I will make a Pillar and give him a Rigidbody.  
What we also need is the Tag. Give the Placeholder Player a player tag.  
You can choose the pre-existing "Player" tag or create a new one.   

Now in our Script we have to clean a bit up since we don't need the cooldown anymore.
```C#
[SerializeField] GameObject Player;
[SerializeField] float Speed;

    private void Update()
    {
        Vector3 direction = Vector3.Normalize(Player.transform.position - transform.position);
        direction.y = 0;
        //Debug.Log($"Direction: {direction}");
        if(direction.x != 0 && direction.z != 0)
        {
            transform.forward = direction;
            transform.position += direction * Time.deltaTime * Speed;
        }
    }
```

We now add a GameObject to our script that will be our Player for now.  
Next up we will add this to our Script:

```C#
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "player") return;
        Destroy(gameObject);
    }
```

We check if the collision is from a Player because maybe we hit the wall, or another Enemy or who knows what. So to be sure we check if the thing we are touching really is the Player. After that we Destroy ourself for now because that's what the Enemy should do if he touches him.  
Oh no! We aren't deleting anything, in fact we aren't even colliding with anything. Well we'll just make a quick fix by copying the collider component and putting it into the base GameObject.  

Is it good? Not really, but it's the GameJam experience!      

**Nice Time to Commit 👁️👁️**  

How about we combine the two branches of the Player and the Enemy and look how those two interact with each other.  
If we want the Enemies to still chase after the Player and get destroyed when they touch them, we need to do some things. One thing would be to appoint every enemy the Player object in the editor. The other thing is to change the tag where the Collider is on the Player.  
Now this should work, but we have to give every enemy the PlayerObject and that is pretty bad, so let's change that.  
We just need the Position of the Player, so we will just save the Position from the Player in his ScriptableObject and let the Enemy read it.  
(Also we could maybe make the Speed of the enemy a bit more random to make it more interesting)  

Add in the PlayerData the Player Position:
```C#
public class PlayerData : ScriptableObject
{
    public Vector3 lookPoint;
    public Vector3 playerPosition; // <--- Add this 
}
```
Then we give the information in the MovementScript:
```C#
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] PlayerData data; // <--- add this
    Vector3 movement = new Vector3();

    private void Update()
    {
        transform.position += movement * Time.deltaTime * speed;
        data.playerPosition = transform.position; // <--- and this
    }
    //....
}
```
And in the end we change the PlayerObject to PlayerData in the Enemy Script
```C#
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] PlayerData playerData; // <--- change this
    [SerializeField] float Speed;

    private void Update()
    {
        Vector3 direction = Vector3.Normalize(playerData.playerPosition - transform.position); // <--- and there
        //...
    }
    //...
}
```
> [!IMPORTANT]
> **DON'T** forget to change the Prefabs from the Player and Enemy, so that they have the Playerdata in them.   

With the Enemy speed you can try it yourself how to make it a bit random, I would recommend to keep the speed and just add or substract a bit from it. 

**Nice Time to Commit 👁️👁️**  

## Shooting at Enemy
How about we add a simple shooting mechanic. For this we do this in another Branch.   
First we will need to add a model to it. (Maybe even a sound if you'd like).   
We than add a simple BulletsScript where we just go along the direction we are looking at, that looks like this:
```C#
[SerializeField] float Speed;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * Speed;
    }
```
Also we have to think how we want to handle the collide.  
In my case I gave the bullet the tag since I manipulate the transform of the body and doing the same thing we did with the enemy won't work, because the collider doesn't safe the rotation.  
We now need the ability to shoot those Bullets, so we should make a new Script for the Player that is just for shooting.  

Which would look like this:
```C#
[SerializeField] GameObject Bullet;

    public void OnShooting(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Instantiate(Bullet,transform.position, Quaternion.identity);
    }
```
In this case we only shoot if we started the Event, meaning we have to let go of the shooting button and then click again. **DON'T** forget to update the Input Manager inside the Player.    
Next up I will change my Enemies behavior so that it destroys itself when it touches the Bullet:
```C#
private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "bullet") return;
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
```
And that would be it right? Not quite right. There is always something wrong like in OnShooting, where we don't give the rotation of the body that is rotating. Or that Our Bullets are living forever if they never hit something.  

Sooo a small update for the shooting code:
```C#
public void OnShooting(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Instantiate(Bullet,transform.position, transform.Find("Body").rotation);
    }
```
And in the script of the Bullet:
```C#
    [SerializeField] float Speed;
    [SerializeField] float LifeSpan;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * Speed;
        if(LifeSpan <= 0) Destroy(gameObject);
        LifeSpan -= Time.deltaTime;
    }
```
**Nice Time to Commit 👁️👁️**  

Alright next up would be to add HP and the UI.
## UI
### Healthbar
First off, we merge the Shooting branch back to our main branch.  
I will continue to work in main branch since we now know how this whole branch things works.  
Let's make a Healthbar. For that we will add a UI-Element, specifically a Panel UI-Element.  

I recommend to use the Anchors to change the size and position of it, since this should make it always the same size and position for every resolution.     
You can duplicate the Panel and change the color. One of them has to be the Background, showing the max-HP, the other will be in the Foreground showing our current-HP. 

Let's make a script for the UI, so that we can adapt the Foreground Panel with our current HP.    
I will show you the code and explain it after. First of we will change our PlayerData script:
```C#
public class PlayerData : ScriptableObject
{
    public Vector3 lookPoint;
    public Vector3 playerPosition;
    public float maxHP, currentHP; // <-- Add this
}
```
After that we need to change the EnemyBehaviour a bit, since now something should happen if we collide with the player:
```C#
private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "bullet")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        if(collision.transform.tag == "player")
        {
            playerData.currentHP--;
            Destroy(gameObject);
            return;
        }
    }
```
Next up, the UI Code and this will need a bit more explanation:
```C#
[RequireComponent(typeof(RectTransform))]
public class HealthUIScript : MonoBehaviour
{
    [SerializeField] PlayerData data;
    [SerializeField] RectTransform BackgroundHPBar;
    RectTransform ForegroundHPBar;
    Vector2 newAnchor;

    private void Start()
    {
        data.maxHP = 10;
        data.currentHP = data.maxHP;
        ForegroundHPBar = GetComponent<RectTransform>();
        newAnchor = new Vector2(0, ForegroundHPBar.anchorMax.y);
    }

    private void Update()
    {
        float HPPercent = Mathf.InverseLerp(0,data.maxHP,data.currentHP);
        float newX = Mathf.Lerp(BackgroundHPBar.anchorMin.x,BackgroundHPBar.anchorMax.x,HPPercent);
        newAnchor.x = newX;
        ForegroundHPBar.anchorMax = newAnchor;
    }
}
```
So first up, we can tell a script if the Object we are attaching to need a specific Component, in this case we need a RectTransform (the thing where we changed the anchors in our Panel-UI)

Next up, we need our PlayerData, and the Background Panel to know what is the maximum lenght for the Foreground Panel.

The next thing is we define our maxHP and our currentHP at the beginning of our UI Life. This is not the best idea, especially if you want to change it afterwards, since you have to search it in the code. Also you would expect it somewhere in the Player Script, not in the UI. But this is just a demo showing you possiblities not the best way to make a game.   

Also to make it a bit easier we put our RectTransform into a variable and the newAnchor position into a Vector2 variable. The reason for that is, we won't change the Y position of the anchor and we can't access the x anchor alone from the label, so this has to do.  

In the Update we have to translate the HP into the UI. In this case we find out the precentage of our current HP to the Max HP. Then use that to make our Panel corresponding to that. In this Case we will use Lerp.  

After that don't forget to add the script and their needed Reference and it should work.    

This should work for now, we can't die, but we'll get there next, but first: **Nice Time to Commit 👁️👁️**  

### Highscore
Alright how about we make the game UI into a prefab? I would recommend to make an empty GameObject and putting the Canvas and EventSystem in it.  
Next up we will write some Text.   
Add in the Canvas "Text - TextMeshPro", there might appear something to install, just install it and continue.  

We will need 4 things:
* A Text that says "Score"
* A Score that we will change during the game
* A Text that says Highscore
* The highscore that we will change at the beginning at the game

Again play a bit with the Anchors of the UI.   
Write at the Text Input the information you need, like for the Score-Text you could write "Score:"  

For things like the actual Score, we can write anything because we will change it with code.   
You can also change the size and font and whatnot inside the TextMeshProUI.  

Next up the script. I will do just one script and give it to our Canvas and call it the ScoreManager.  
But before we need to change a bit in the other scripts:

First up we will need to add the following in the `PlayerData`
```C#
public class PlayerData : ScriptableObject
{
    public Vector3 lookPoint;
    public Vector3 playerPosition;
    public float maxHP, currentHP;
    public int highScore, score; // <----- this right here
}
```
Next up the `EnemyBehaivour` should increase our score if he dies
```C#
private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "bullet")
        {
            playerData.score++; // <----- add this
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        if(collision.transform.tag == "player")
        {
            playerData.currentHP--;
            Destroy(gameObject);
            return;
        }
    }
```
and finally our `ScoreManager`
```C#
    [SerializeField] PlayerData data;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    private void Start()
    {
        highScoreText.text = data.highScore.ToString();
        data.score = 0;
    }

    private void Update()
    {
        scoreText.text = data.score.ToString();
        if (data.currentHP > 0) return;
        if(data.highScore < data.score)
        {
            data.highScore = data.score;
        }

    }
```
> [!TIP]
> If the TextMeshProUGUI doesn't work, than click on it and Press ALT+. to open the hint thingy. There should be a way to import TMPro.    

We will need the `PlayerData` for the high- & score and our two UI-Text we want to change.  
We begin the UI with overwriting the highscore Text with the last highscore we wrote (in this case it will be 0 because that's the default value for int).  

Next up in the update we will write the score and look if we are still alive.  

If we are dead we will look if we made a new record, if so we overwrite the highscore.  

Don't forget to add the component to the script and this should be it. We are not done yet, there is just a small bit to make it feel like a game, but before that:  

**Nice Time to Commit 👁️👁️**    

So to make it more like a game we need to have a GameOver screen and make the enemy spawn infinity.    
Let's begin with the enemy spawning.  

## The Game Scene
I am going to make a new scene, the Game Scene. In here we put our important stuff first, a Floor, GameUI and our Player.    

### Enemy Spawning
We will now make a big box and call it the arena, you can make it invisible (by deactivating the Mesh Renderer) because we will just teleport our Player if he goes outside the box.  

To do so we will also check the "Is Trigger" on in the Box Collider.    
Let's focus first on the Enemy and then make the Player trapped in this box.  

We will need a new Script, call it EnemySpawning and give it the arena.  
```C#
[SerializeField] GameObject Enemy;
[SerializeField] float cooldown;

float remainingTime;
float randomX,randomZ;

Vector3 randomPosition;

private void Update()
{
    if (remainingTime <= 0)
    {
        randomX = Random.Range(-transform.localScale.x/2, transform.localScale.x/2);
        randomZ = Random.Range(-transform.localScale.z/2, transform.localScale.z/2);
        randomPosition = new Vector3(randomX,0,randomZ);
        
        Instantiate(Enemy, randomPosition, Quaternion.identity);

        remainingTime = cooldown;
    }
    remainingTime -= Time.deltaTime;
}
```
We'll need the scale of the box and to find it that out we can either use lossyScale or localScale. I chose localScale because it shouldn't matter since the scale isn't being manipulated by another object (like a parent).     
Also we need half of the scale because we are at Position 0/0 and if we have a scale of 50 that means we are between -25 and 25.  

**Nice Time to Commit 👁️👁️**  

### Keeping the Player inside
To keep the player inside we need a new Script for the Arena, I called it `KeepingPlayerInBound`.  
```C#
Vector3 newPosition;

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "player") return;
        Transform player = other.gameObject.transform.parent;
        Vector3 halfScale = transform.localScale / 2;

        // //bad code
        //if (player.position.x < -halfScale.x)
        //    newPosition.x = halfScale.x - 0.1f;
        //if (player.position.x > halfScale.x / 2)
        //    newPosition.x = -halfScale.x / 2 + 0.1f;
        //if (player.position.z < -halfScale.z / 2)
        //    newPosition.z = halfScale.y / 2 - 0.1f;
        //if (player.position.z > halfScale.z / 2)
        //    newPosition.z = -halfScale.z / 2 + 0.1f;

        // works because of Position (0,0,0) of Arena
        // also a hard teleport instead of wrapping
        newPosition.x = -Mathf.Clamp(player.position.x, -halfScale.x + 0.1f, halfScale.x - 0.1f);
        newPosition.z = -Mathf.Clamp(player.position.z, -halfScale.z + 0.1f, halfScale.z - 0.1f);

        // Wrapping
        //newPosition.x = Mathf.Repeat(player.position.x + halfScale.x,transform.localScale.x) - halfScale.x;
        //newPosition.z = Mathf.Repeat(player.position.z + halfScale.z, transform.localScale.z) - halfScale.z;

        player.position = newPosition;
    }
```
It would been a short code but I wanted to show some examples.  

The first one is the typical example if you are tired or new, just a lot of ifs. It works, it just doesn't look great.    

The second one is much better, it is just two rows of code. Buuut it only works because we have a mirroring position value thanks to the arena being in the coordination (0,0,0). If it wouldn't be the case, this wouldn't work that great.    

So the best case would be to do a wrapper. The `Mathf.Repeat()` is just like modulo but you can't go negative and it just from 0 to our max scale and being negative means we start from the highest number.    

Now the only thing left is to make a Game Over screen. Normally this would be a great time to commit, but I will quickly just do it.    

## The Game Over Screen
- Create a new Scene and put a new UI there.  
- Create Panels, Text and most important a button.   

I will show our score and our highscore and also make a new script called `GameOverScript`.  
 
You can copy most of `ScoreManager` since it's pretty much the same, just without the player and overwriting anything.  

First of in the `ScoreManager` we need to add something, our new scene:
```C#
private void Update()
    {
        scoreText.text = data.score.ToString();
        if (data.currentHP > 0) return;
        if(data.highScore < data.score)
        {
            data.highScore = data.score;
        }
        SceneManager.LoadScene("GameOver"); //<--- Add this
    }
```
Next up the GameOver Script looks like this:
```C#
    [SerializeField] PlayerData data;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    private void Start()
    {
        highScoreText.text = data.highScore.ToString();
        scoreText.text = data.score.ToString();
    }

    public void OnButtonClick()
    {
        SceneManager.LoadScene("Game");
    }
```
Now the only thing we need to do is, to give it to our Canvas and connect the thing. And then go to our Button and there is plus below OnClick, we press this, give it our Script and now it will fire the Methode when we click the button.    

To make it work we still need to do one final thing. We need to define our Scenes. If you go to File->Build Profiles you'll find many cool things, but we want to open the Scene List. Now open one Scene after the other and add them inside. After that, it should work (as long as you have the same name as in the code).  