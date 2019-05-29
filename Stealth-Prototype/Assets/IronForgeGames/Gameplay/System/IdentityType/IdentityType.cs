using UnityEngine;

namespace AH.Max.System
{
	[CreateAssetMenu(fileName = "New Identity Type", menuName = "CompanyName/IdentityType", order = 1)]
	public class IdentityType : ScriptableObject 
	{
		///<Summary>
		///The name of the identity type
		///</Summary>
		public string name;
		
		///<Summary>
		///The type of entity it is
		///</Summary>
		public UsageType type;

		///<Summary>
		///The prefab associated with the Identity
		///</Summary>
		public GameObject prefab;

        /// <summary>
        /// Provided logic for hierarchies and grouping. If possible if you can logically groupt idenities together and give them a parent you should.
        /// </summary>
        public IdentityType parent;

        /// <summary>
        /// A method that can be used to check if one Identity Type is a decendent of another
        /// This is helpful for grouping together multiple identities. 
        /// </summary>
        /// <param name="decendant"></param>
        /// <param name="ancestor"></param>
        /// <returns></returns>
        public static bool IsDesendantOf(IdentityType decendant, IdentityType ancestor)
        {
            if(decendant.parent == null)
            {
                return false;
            }

            if(decendant == ancestor)
            {
                return true;
            }

            IdentityType currentCheck = decendant.parent;

            while(true)
            {
                if(currentCheck == ancestor)
                {
                    return true;
                }

                currentCheck = currentCheck.parent;

                if(currentCheck == null)
                {
                    return false;
                }
            }
        }
	}
}
