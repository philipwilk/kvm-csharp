using MySql.Data.MySqlClient;

namespace Main
{

  partial class sql
  {
    public MySqlConnection conn { get; }

    static string hosts_table = "CREATE TABLE IF NOT EXISTS hosts (Uuid uuid NOT NULL UNIQUE, FriendlyName text NOT NULL, Memory bigint unsigned NOT NULL, Cpus smallint NOT NULL, Arch text NOT NULL, Ip text, IsManager boolean NOT NULL, State text, primary key (Uuid));";
    static string vdisks_table = "CREATE TABLE IF NOT EXISTS vdisks (Uuid uuid NOT NULL UNIQUE, Path text NOT NULL, Host Uuid, primary key (Uuid), foreign key (Host) references hosts(Uuid) on update cascade on delete set NULL);";
    static string vdevs_table = "CREATE TABLE IF NOT EXISTS vdevs (Uuid uuid NOT NULL UNIQUE, Address bigint NOT NULL, Host Uuid, Type text NOT NULL, AddressLength bigint NOT NULL, isSystem bool NOT NULL, primary key (Uuid), foreign key (Host) references hosts(Uuid) on update cascade on delete set NULL);";
    static string templates_table = "CREATE TABLE IF NOT EXISTS vm_templates (Uuid uuid NOT NULL UNIQUE, FriendlyName text NOT NULL, Memory bigint unsigned NOT NULL, Vcpus smallint NOT NULL, BootDeviceType text, Arch text NOT NULL, primary key (Uuid));";
    static string vms_table = "CREATE TABLE IF NOT EXISTS vms (Uuid uuid NOT NULL UNIQUE, FriendlyName text NOT NULL, Memory bigint unsigned NOT NULL, Vcpus smallint NOT NULL, BootDeviceType text, Template uuid, Arch text NOT NULL, primary key (Uuid), foreign key (Template) references vm_templates(Uuid));";
    static string assigned_vdisk_table = "CREATE TABLE IF NOT EXISTS assigned_vdisks (Uuid uuid NOT NULL UNIQUE, BootOrder int, VmUuid uuid, VDiskUuid uuid, primary key (Uuid), foreign key (VmUuid) references vms(Uuid) on update cascade on delete set NULL, foreign key (VDiskUuid) references vdisks(Uuid) on update cascade on delete set NULL);";
    static string assigned_vdev_table = "CREATE TABLE IF NOT EXISTS assigned_vdevs (Uuid uuid NOT NULL UNIQUE, DeviceOrder int, VmUuid uuid, VDevUuid uuid, primary key (Uuid), foreign key (VmUuid) references vms(Uuid) on update cascade on delete set NULL, foreign key (VDevUuid) references vdevs(Uuid) on update cascade on delete set NULL);";
    static string running_vms = "CREATE TABLE IF NOT EXISTS running_vms (Uuid uuid NOT NULL UNIQUE, State text, Host uuid, IsOrphan bool, VmUuid uuid, primary key (Uuid), foreign key (Host) references hosts(Uuid), foreign key (VmUuid) references vms(Uuid));";

    /// <summary>
    /// Create connection to mysql database and open it.
    /// The user needs Mysql's global create new database and table permission to create the database.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="userid"></param>
    /// <param name="password"></param>
    public sql(string server, string userid, string password)
    {
      var connection_string = String.Format("server={0};user={1};port=3306;password={2}", server, userid, password);
      conn = new MySqlConnection(connection_string);
      conn.Open();

    }

    ~sql()
    {
      conn.Close();
    }

    public void init_db()
    {
      var str = "CREATE DATABASE IF NOT EXISTS kvm CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci";
      MySqlCommand comm = new MySqlCommand(str, conn);
      int first_run = comm.ExecuteNonQuery(); // 1 if yes, 0 if no
      conn.ChangeDatabase("kvm");

      MySqlCommand create_hosts_table_table = new MySqlCommand(hosts_table, conn);
      create_hosts_table_table.ExecuteNonQuery();

      MySqlCommand create_vdisks_table = new MySqlCommand(vdisks_table, conn);
      create_vdisks_table.ExecuteNonQuery();

      MySqlCommand create_vdevs_table = new MySqlCommand(vdevs_table, conn);
      create_vdevs_table.ExecuteNonQuery();

      MySqlCommand create_templates_table = new MySqlCommand(templates_table, conn);
      create_templates_table.ExecuteNonQuery();

      MySqlCommand create_vms_table = new MySqlCommand(vms_table, conn);
      create_vms_table.ExecuteNonQuery();

      MySqlCommand create_assigned_vdisk_table = new MySqlCommand(assigned_vdisk_table, conn);
      create_assigned_vdisk_table.ExecuteNonQuery();

      MySqlCommand create_assigned_vdev_table = new MySqlCommand(assigned_vdev_table, conn);
      create_assigned_vdev_table.ExecuteNonQuery();

      MySqlCommand create_running_vms_table = new MySqlCommand(running_vms, conn);
      create_running_vms_table.ExecuteNonQuery();
    }

    // Create host structure in db
    public void create_host(host self)
    {
      string sql_str = String.Format("INSERT IGNORE INTO hosts (Uuid, FriendlyName, Memory, Cpus, Arch, isManager, State) VALUES ('{0}', '{1}', {2}, {3}, '{4}', {5}, '{6}');", self.uuid, self.friendly_name, self.memory, self.threads, self.arch, self.is_manager, self.state);

      MySqlCommand create_host = new MySqlCommand(sql_str, conn);
      create_host.ExecuteNonQuery();

      return;
    }

  }
}