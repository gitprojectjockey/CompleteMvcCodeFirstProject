/// <reference path="../scripts/typings/jquery/jquery.d.ts" />
var Instructor;
(function (Instructor) {
    var Edit;
    (function (Edit) {
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
            function InitCourses() {
                var codeList = GetSelectedCourses();
                $('#selectedCourseList').empty();
                codeList.forEach(function (c) {
                    $('#selectedCourseList').append('<h4>' + c.courseInfo() + '</h4>');
                });
            }
            InitCourses();
        }
        Edit.init = init;
    })(Edit = Instructor.Edit || (Instructor.Edit = {}));
})(Instructor || (Instructor = {}));
$(document).ready(function () {
    if ($('#0929AA6C-6367-414E-87FD-F89151019A7B').length > 0) {
        Instructor.Edit.init();
    }
});
//# sourceMappingURL=instructor-edit.js.map