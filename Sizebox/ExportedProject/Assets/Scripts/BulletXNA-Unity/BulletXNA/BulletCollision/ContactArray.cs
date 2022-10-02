using System;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class ContactArray : ObjectArray<GIM_CONTACT>
	{
		public const int MAX_COINCIDENT = 8;

		public ContactArray()
			: base(64)
		{
		}

		private void PushContact(ref IndexedVector3 point, ref IndexedVector3 normal, float depth, int feature1, int feature2)
		{
			Add(new GIM_CONTACT(ref point, ref normal, depth, feature1, feature2));
		}

		private void PushContact(ref IndexedVector3 point, ref IndexedVector4 normal, float depth, int feature1, int feature2)
		{
			IndexedVector3 normal2 = new IndexedVector3(normal.X, normal.Y, normal.Z);
			Add(new GIM_CONTACT(ref point, ref normal2, depth, feature1, feature2));
		}

		public void PushTriangleContacts(GIM_TRIANGLE_CONTACT tricontact, int feature1, int feature2)
		{
			for (int i = 0; i < tricontact.m_point_count; i++)
			{
				PushContact(ref tricontact.m_points[i], ref tricontact.m_separating_normal, tricontact.m_penetration_depth, feature1, feature2);
			}
		}

		public void MergeContacts(ContactArray contacts)
		{
			MergeContacts(contacts, true);
		}

		public void MergeContacts(ContactArray contacts, bool normal_contact_average)
		{
			Clear();
			if (contacts.Count == 0)
			{
				return;
			}
			if (contacts.Count == 1)
			{
				Add(contacts[0]);
				return;
			}
			ObjectArray<CONTACT_KEY_TOKEN> objectArray = new ObjectArray<CONTACT_KEY_TOKEN>();
			objectArray.Capacity = contacts.Count;
			for (int i = 0; i < contacts.Count; i++)
			{
				objectArray.Add(new CONTACT_KEY_TOKEN(contacts[i].CalcKeyContact(), i));
			}
			objectArray.Sort(CONTACT_KEY_TOKEN.SortPredicate);
			int num = 0;
			IndexedVector3[] array = new IndexedVector3[8];
			uint num2 = objectArray[0].m_key;
			uint num3 = 0u;
			Add(contacts[objectArray[0].m_value]);
			GIM_CONTACT gIM_CONTACT = base[0];
			int index = 0;
			for (int j = 1; j < objectArray.Count; j++)
			{
				num3 = objectArray[j].m_key;
				GIM_CONTACT gIM_CONTACT2 = contacts[objectArray[j].m_value];
				if (num2 == num3)
				{
					if (gIM_CONTACT.m_depth - 1E-05f > gIM_CONTACT2.m_depth)
					{
						base[index] = gIM_CONTACT2;
						num = 0;
					}
					else if (normal_contact_average && Math.Abs(gIM_CONTACT.m_depth - gIM_CONTACT2.m_depth) < 1E-05f && num < 8)
					{
						array[num] = gIM_CONTACT2.m_normal;
						num++;
					}
				}
				else
				{
					if (normal_contact_average && num > 0)
					{
						gIM_CONTACT.InterpolateNormals(array, num);
						num = 0;
					}
					Add(gIM_CONTACT2);
					index = base.Count - 1;
					gIM_CONTACT = base[index];
				}
				num2 = num3;
			}
		}

		public void MergeContactsUnique(ContactArray contacts)
		{
			Clear();
			if (contacts.Count == 0)
			{
				return;
			}
			if (contacts.Count == 1)
			{
				Add(contacts[0]);
				return;
			}
			GIM_CONTACT gIM_CONTACT = contacts[0];
			for (int i = 1; i < contacts.Count; i++)
			{
				gIM_CONTACT.m_point += contacts[i].m_point;
				gIM_CONTACT.m_normal += contacts[i].m_normal * contacts[i].m_depth;
			}
			float num = 1f / (float)contacts.Count;
			gIM_CONTACT.m_point *= num;
			gIM_CONTACT.m_normal *= num;
			gIM_CONTACT.m_depth = gIM_CONTACT.m_normal.Length();
			gIM_CONTACT.m_normal /= gIM_CONTACT.m_depth;
		}
	}
}
