import 'package:cross_ways/components/animation_route.dart';
import 'package:cross_ways/views/main_menu_view.dart';
import 'package:flutter/material.dart';
import 'package:material_symbols_icons/symbols.dart';
import 'log_in_view.dart';
import 'package:cross_ways/database/create_database_with_user.dart';

// Використання Introduce Parameter Object для передачі даних користувача
class User {
  final String nickname;
  final String name;
  final String gender;
  final DateTime birthday;

  User({
    required this.nickname,
    required this.name,
    required this.gender,
    required this.birthday,
  });
}

// Використання Replace Constructor with Factory Method для створення користувача
class UserFactory {
  static User createUser(String nickname, String name, String gender, DateTime birthday) {
    return User(
      nickname: nickname,
      name: name,
      gender: gender,
      birthday: birthday,
    );
  }
}

class UserRegScreen extends StatefulWidget {
  @override
  _UserRegScreenState createState() => _UserRegScreenState();
}

class _UserRegScreenState extends State<UserRegScreen> {
  DateTime? birthday;
  String? selectedGender;
  String? nickname;
  String? name;

  TextEditingController _dateController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color.fromARGB(255, 188, 188, 176),
      body: Stack(
        children: [
          Positioned(
            top: 40,
            left: 20,
            child: IconButton(
              icon: const Icon(
                Symbols.arrow_back_2,
                fill: 1,
                color: Color.fromARGB(255, 135, 100, 71),
                size: 32,
              ),
              onPressed: () {
                Navigator.pushReplacement(
                    context, FadePageRoute(page: LogInScreen()));
              },
            ),
          ),
          Center(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 30),
              child: Container(
                width: 330,
                height: 560,
                decoration: BoxDecoration(
                  color: const Color.fromARGB(255, 231, 229, 225),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Column(
                  children: [
                    const Padding(
                      padding: EdgeInsets.only(top: 40, bottom: 20),
                      child: Text(
                        'Sign Up',
                        style: TextStyle(
                          color: Color.fromARGB(255, 135, 100, 71),
                          fontSize: 40,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    // Поле NickName
                    buildTextField('NickName', (value) => nickname = value),
                    // Поле Name
                    buildTextField('Name', (value) => name = value),
                    // Поле Birthday
                    buildDateField(),
                    // Вибір Гендеру
                    buildGenderSelection(),
                    // Кнопка Continue
                    buildContinueButton(context),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Винесення логіки полів введення в окремий метод
  Widget buildTextField(String label, Function(String) onChanged) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.grey.withOpacity(0.3),
              blurRadius: 4,
              offset: const Offset(2, 2),
            ),
          ],
        ),
        child: TextField(
          onChanged: onChanged,
          decoration: InputDecoration(
            labelText: label,
            labelStyle: const TextStyle(color: Color.fromARGB(255, 135, 100, 71)),
            contentPadding: const EdgeInsets.symmetric(horizontal: 20),
            border: InputBorder.none,
          ),
        ),
      ),
    );
  }

  Widget buildDateField() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.grey[200],
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.grey.withOpacity(0.3),
              blurRadius: 4,
              offset: const Offset(2, 2),
            ),
          ],
        ),
        child: TextField(
          controller: _dateController,
          readOnly: true,
          decoration: const InputDecoration(
            labelText: 'Pick a date',
            labelStyle: TextStyle(color: Color.fromARGB(255, 135, 100, 71)),
            contentPadding: EdgeInsets.symmetric(horizontal: 20),
            suffixIcon: Icon(Icons.calendar_today, color: Color.fromARGB(255, 135, 100, 71)),
            border: InputBorder.none,
          ),
          onTap: () async {
            birthday = await showDatePicker(
              context: context,
              initialDate: DateTime.now(),
              firstDate: DateTime(1900),
              lastDate: DateTime.now(),
              builder: (BuildContext context, Widget? child) {
                return Theme(
                  data: ThemeData.light().copyWith(
                    primaryColor: const Color.fromARGB(255, 135, 100, 71),
                    hintColor: const Color.fromARGB(255, 135, 100, 71),
                    colorScheme: const ColorScheme.light(primary: Color.fromARGB(255, 135, 100, 71)),
                    buttonTheme: const ButtonThemeData(textTheme: ButtonTextTheme.primary),
                  ),
                  child: child!,
                );
              },
            );
            if (birthday != null) {
              _dateController.text = "${birthday!.toLocal()}".split(' ')[0];
            }
          },
        ),
      ),
    );
  }

  Widget buildGenderSelection() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
      child: Container(
        width: 330,
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.grey.withOpacity(0.3),
              blurRadius: 4,
              offset: const Offset(2, 2),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Padding(
              padding: EdgeInsets.only(left: 10),
              child: Text(
                'Gender',
                style: TextStyle(
                  color: Color.fromARGB(255, 135, 100, 71),
                  fontSize: 16,
                ),
              ),
            ),
            Row(
              children: [
                buildRadioOption('Male'),
                buildRadioOption('Female'),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget buildRadioOption(String gender) {
    return Row(
      children: [
        Radio<String>(
          value: gender,
          groupValue: selectedGender,
          activeColor: const Color.fromARGB(255, 135, 100, 71),
          onChanged: (String? value) {
            setState(() {
              selectedGender = value;
            });
          },
        ),
        Text(
          gender,
          style: TextStyle(
            color: selectedGender == gender ? const Color.fromARGB(255, 135, 100, 71) : Colors.black,
          ),
        ),
      ],
    );
  }

  Widget buildContinueButton(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 30),
      child: SizedBox(
        width: 250,
        height: 50,
        child: ElevatedButton(
          onPressed: () {
            validateAndSubmit(context);
          },
          style: ElevatedButton.styleFrom(
            backgroundColor: const Color.fromARGB(255, 92, 109, 103),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20),
            ),
            elevation: 5,
            shadowColor: Colors.black45,
          ),
          child: const Text(
            'Continue',
            style: TextStyle(color: Colors.white, fontSize: 16),
          ),
        ),
      ),
    );
  }

  void validateAndSubmit(BuildContext context) {
    final validator = UserValidator(
      nickname: nickname,
      name: name,
      birthday: birthday,
      selectedGender: selectedGender,
    );

    if (!validator.isFormComplete()) {
      showError(context, 'Please fill in all fields');
    } else if (!validator.isEighteenOrOlder()) {
      showError(context, 'Your age is under 18');
    } else {
      final user = UserFactory.createUser(nickname!, name!, selectedGender!, birthday!);
      addUser(user.nickname, user.name, user.gender, user.birthday);
      Navigator.pushReplacement(context, PushPageRoute(page: MainMenuView()));
    }
  }

  void showError(BuildContext context, String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message), backgroundColor: Colors.red),
    );
  }
}

// Валидація (Replace Method with Method Object)
class UserValidator {
  final String? nickname;
  final String? name;
  final DateTime? birthday;
  final String? selectedGender;

  UserValidator({
    required this.nickname,
    required this.name,
    required this.birthday,
    required this.selectedGender,
  });

  bool isFormComplete() {
    return nickname != null && nickname!.isNotEmpty &&
           name != null && name!.isNotEmpty &&
           birthday != null &&
           selectedGender != null;
  }

  bool isEighteenOrOlder() {
    final today = DateTime.now();
    final age = today.year - birthday!.year;

    if (today.month < birthday!.month ||
        (today.month == birthday!.month && today.day < birthday!.day)) {
      return age >= 18 - 1;
    }

    return age >= 18;
  }
}
