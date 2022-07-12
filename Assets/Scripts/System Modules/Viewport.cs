using UnityEngine;

public class Viewport : Singleton<Viewport>
{
   float minX;
   float maxX;
   float minY;
   float maxY; 
   float middleX;

   public float MaxX => maxX;

   void Start()    //将视口坐标转化为世界坐标
   {
       Camera mainCamera = Camera.main;

       Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f,0f));
       Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f,1f));

       middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0f,0f)).x;

        minX=bottomLeft.x;
        minY=bottomLeft.y;
        maxX=topRight.x;
        maxY=topRight.y;

   }

   public Vector3 PlayerMoveablePosition(Vector3 playerPosition,float paddingX, float paddingY)   //限定玩家移动位置
   {
       Vector3 position = Vector3.zero;

       position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
       position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

       return position;
   }

   public Vector3 RandomEnemySpawnPosition(float paddingX, float paddingY)
   {
       Vector3 position = Vector3.zero;

       position.x = maxX+paddingX;
       position.y = Random.Range(minY+paddingY,maxY-paddingY);

       return position;
   }

    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(middleX, maxX-paddingX);
        position.y = Random.Range(minY+paddingY, maxY-paddingY);

        return position;
    }

    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX+paddingX, maxX-paddingX);
        position.y = Random.Range(minY+paddingY, maxY-paddingY);

        return position;
    }

}
