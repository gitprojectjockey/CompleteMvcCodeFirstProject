/// <reference path="../scripts/typings/jquery/jquery.d.ts" />
var Instructor;
(function (Instructor) {
    var CreateAndEdit;
    (function (CreateAndEdit) {
        function init() {
            var Course = (function () {
                function Course(code, name) {
                    this.code = code;
                    this.name = name;
                }
                Course.prototype.courseInfo = function () {
                    return this.code + " " + this.name;
                };
                return Course;
            }());
            //Events
            $(".course tr td input").change(function (e) {
                var codeList = GetSelectedCourses();
                $('#selectedCourseList').empty();
                codeList.forEach(function (c) {
                    $('#selectedCourseList').append('<h4>' + c.courseInfo() + '</h4>');
                });
            });
            $('#btnInstructor-Create-Save').click(function (e) {
                var atleastOneIsChecked = ValidateCoursesChecked();
                return atleastOneIsChecked;
            });
            //functions
            function GetSelectedCourses() {
                var list = [];
                $('.course input:checkbox:checked').each(function () {
                    var code = $(this).parent().siblings().eq(0).text().trim();
                    var name = $(this).parent().siblings().eq(1).text().trim();
                    var course = new Course(code, name);
                    course.code = code;
                    list.push(course);
                });
                return list;
            }
            function ValidateCoursesChecked() {
                var checked = false;
                $('.course input:checkbox:checked').each(function () {
                    checked = true;
                });
                return checked;
            }
            function InitCourses() {
                var codeList = GetSelectedCourses();
                $('#selectedCourseList').empty();
                codeList.forEach(function (c) {
                    $('#selectedCourseList').append('<h4>' + c.courseInfo() + '</h4>');
                });
            }
            InitCourses();
        }
        CreateAndEdit.init = init;
    })(CreateAndEdit = Instructor.CreateAndEdit || (Instructor.CreateAndEdit = {}));
})(Instructor || (Instructor = {}));
$(document).ready(function () {
    //only init instructor-create-edit.js if it is the instructor.edit or instrunctor.create pages
    if ($('#0929AA6C-6367-414E-87FD-F89151019A7B').length > 0 || $('#AB7BEE7D-141F-4C23-915A-A4EDD8346790').length > 0) {
        Instructor.CreateAndEdit.init();
    }
});
//# sourceMappingURL=instructor-create-edit.js.map