namespace TranningManagement.Model
{
    public class ClassDetailsDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public string TeacherName { get; set; }
        public List<StudentDTO> Students { get; set; }
        public List<SchedulesDTO> Schedules { get; set; }
        public List<LearningMaterialsDTO> LearningMaterials { get; set; }
    }
}
    
