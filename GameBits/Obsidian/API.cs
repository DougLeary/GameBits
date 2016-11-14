using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
	public class ObsidianUser
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string ProfileUrl { get; set; }
		public string AvatarImageUrl { get; set; }

		public ObsidianUser()
		{
			Id = String.Empty;
			Username = String.Empty;
			ProfileUrl = String.Empty;
			AvatarImageUrl = String.Empty;
		}

		public ObsidianUser FromJson(Dictionary<string, string> json)
		{
			ObsidianUser user = new ObsidianUser();
			user.Id = getJson(json, "id");
			user.Username = getJson(json, "username");
			user.ProfileUrl = getJson(json, "profile_url");
			user.AvatarImageUrl = getJson(json, "avatar_image_url");
			return user;
		}

		private string getJson(Dictionary<string, string> json, string key)
		{
			if (json.ContainsKey(key))
				return json[key];
			else
				return String.Empty;
		}
	}

	public class Campaign
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string CampaignUrl { get; set; }
		public string Visibility { get; set; }
		public ObsidianUser GameMaster { get; set; }
		public string PlayStatus { get; set; }
		public List<ObsidianUser> Players { get; }
		public List<ObsidianUser> Fans { get; }
		public bool LookingForPlayers { get; set; }
		public DateTime DateCreated { get; }
		public DateTime DateUpdated { get; }

		public 
	}

	public class Location
	{
		public decimal Latitude { get; set; }
		public decimal Longtitude { get; set; }
	}

}


