using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteManager : MonoBehaviour
{
    [SerializeField] private List<CharacterSprite> m_sprites;
    [SerializeField] private Transform m_playerPos;
    [SerializeField] private List<Transform> m_npcPos;

    private static int NUMBER_NPC = 0;
    void Awake()
    {
        m_sprites = new List<CharacterSprite>();
        NUMBER_NPC = 0;
    }
    
    public CharacterSprite RequestCharacterSprite(GameObject _prefab)
    {
        GameObject spriteInstance = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
        CharacterSprite sprite = spriteInstance.GetComponent<CharacterSprite>();
        
        m_sprites.Add(sprite);
        return sprite;
    }

    public void RemoveCharacterSprite(CharacterSprite _sprite)
    {
        m_sprites.Remove(_sprite);
        if(_sprite != null) Destroy(_sprite.gameObject);
    }

    public CharacterSprite RequestPlayerSprite(GameObject _prefab)
    {
        
        CharacterSprite sprite = RequestCharacterSprite(_prefab);
        sprite.transform.SetParent(m_playerPos);
        sprite.transform.localPosition = Vector3.zero;
        return sprite;
    }
    public CharacterSprite RequestNPCSprite(GameObject _prefab)
    {
        
        CharacterSprite sprite = RequestCharacterSprite(_prefab);
        sprite.transform.SetParent(m_npcPos[NUMBER_NPC]);
        sprite.transform.localPosition = Vector3.zero;
        NUMBER_NPC++;
        return sprite;
    }
}
